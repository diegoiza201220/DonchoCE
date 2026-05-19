using ComprobantesElectronicos.Utils;
using EFModel.Models;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace ComprobantesElectronicos.Services
{
    public class FirmaElectronicaService
    {
        private readonly IConfiguration _config;

        // ── URIs correctas W3C y ETSI ────────────────────────────────────────
        // Estas son las URIs COMPLETAS y EXACTAS que exigen el estándar XAdES-BES
        // y que .NET usa internamente para resolver los algoritmos criptográficos.
        // El error CryptographicException ocurría porque las URIs estaban truncadas.
        private const string UriRsaSha1 = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
        private const string UriRsaSha256 = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
        private const string UriSha1 = "http://www.w3.org/2000/09/xmldsig#sha1";
        private const string UriSha256 = "http://www.w3.org/2001/04/xmlenc#sha256";
        private const string UriXadesNs = "http://uri.etsi.org/01903/v1.3.2#";
        private const string UriXadesSignedProps = "http://uri.etsi.org/01903#SignedProperties";
        private const string UriDsigNs = "http://www.w3.org/2000/09/xmldsig#";

        public FirmaElectronicaService(IConfiguration config) => _config = config;

        private X509Certificate2 CargarCertificado()
        {
            try
            {
                var rutaCertificado = _config["FirmaElectronica:RutaCertificado"];
                var clave = _config["FirmaElectronica:Clave"];

                var certificado = X509CertificateLoader.LoadPkcs12FromFile(
                    rutaCertificado!,
                    clave,
                    X509KeyStorageFlags.Exportable
                );
                return certificado;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cargando certificado: {ex.Message}");
                throw;
            }
        }

        public string FirmarXmlSRI(string xmlSinFirmar)
        {
            // Registrar RSA+SHA256 con su URI W3C correcta para que .NET lo resuelva
            CryptoConfig.AddAlgorithm(
                typeof(RsaPkcs1Sha256SignatureDescription),
                UriRsaSha256
            );

            var certificado = CargarCertificado();
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml(xmlSinFirmar);

            var elementoRaiz = xmlDoc.DocumentElement!;
            var idDocumento = "comprobante";
            elementoRaiz.SetAttribute("id", idDocumento);

            var signatureId = $"Signature-{Guid.NewGuid():N}";
            var signedPropsId = $"SignedProperties-{signatureId}";

            // Hash del certificado en SHA-1 (requerido por XAdES-BES v1.3.2 del SRI)
            byte[] certHashBytes;
            using (var sha1 = SHA1.Create())
                certHashBytes = sha1.ComputeHash(certificado.RawData);
            var certHashB64 = Convert.ToBase64String(certHashBytes);

            var certIssuer = certificado.IssuerName.Name;
            var certSerial = certificado.SerialNumber;

            // Bloque XAdES con URIs completas y correctas
            var xadesXml = $@"<xades:QualifyingProperties
                    xmlns:xades=""{UriXadesNs}""
                    xmlns:ds=""{UriDsigNs}""
                    Target=""#{signatureId}"">
                <xades:SignedProperties Id=""{signedPropsId}"">
                    <xades:SignedSignatureProperties>
                        <xades:SigningTime>{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}</xades:SigningTime>
                        <xades:SigningCertificate>
                            <xades:Cert>
                                <xades:CertDigest>
                                    <ds:DigestMethod Algorithm=""{UriSha1}""/>
                                    <ds:DigestValue>{certHashB64}</ds:DigestValue>
                                </xades:CertDigest>
                                <xades:IssuerSerial>
                                    <ds:X509IssuerName>{certIssuer}</ds:X509IssuerName>
                                    <ds:X509SerialNumber>{certSerial}</ds:X509SerialNumber>
                                </xades:IssuerSerial>
                            </xades:Cert>
                        </xades:SigningCertificate>
                    </xades:SignedSignatureProperties>
                </xades:SignedProperties>
            </xades:QualifyingProperties>";

            var xadesDoc = new XmlDocument();
            xadesDoc.LoadXml(xadesXml);

            var signedXml = new SignedXmlConId(xmlDoc)
            {
                SigningKey = certificado.GetRSAPrivateKey()
            };
            signedXml.Signature.Id = signatureId;
            signedXml.SignedInfo.SignatureMethod = UriRsaSha1;

            // ── CORRECCIÓN "Malformed reference element" ──────────────────────
            // El nodo XAdES debe existir DENTRO del documento XML antes de que
            // SignedXml calcule el digest de la referencia refXades.
            // Se inserta como hijo temporal del elemento raíz; al llamar
            // GetXml() la firma lo mueve al lugar correcto dentro de <Signature>.
            var xadesNodoImportado = xmlDoc.ImportNode(xadesDoc.DocumentElement!, true);
            xmlDoc.DocumentElement!.AppendChild(xadesNodoImportado);

            // Referencia al documento principal
            var refDocumento = new Reference($"#{idDocumento}")
            {
                DigestMethod = UriSha1
            };
            refDocumento.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            signedXml.AddReference(refDocumento);

            // Referencia a SignedProperties — tipo obligatorio en XAdES-BES
            // Al estar ya en el árbol XML, SignedXml puede resolver y hashear el nodo
            var refXades = new Reference($"#{signedPropsId}")
            {
                Type = UriXadesSignedProps,
                DigestMethod = UriSha1
            };
            signedXml.AddReference(refXades);

            // KeyInfo con el certificado completo
            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(certificado));
            signedXml.KeyInfo = keyInfo;

            // Adjuntar el bloque XAdES también como DataObject de la firma
            var dataObject = new DataObject();
            dataObject.Data = xadesNodoImportado.SelectNodes(".")!;
            signedXml.AddObject(dataObject);

            //xmlDoc.DocumentElement!.RemoveChild(xadesNodoImportado);

            //Añadir el<Signature> completo al documento

            //xmlDoc.DocumentElement!.AppendChild(xmlDoc.ImportNode(xmlFirma, true));

            try
            {
                signedXml.ComputeSignature();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al calcular firma: {ex.Message}");
                throw;
            }

            var xmlFirma = signedXml.GetXml();

            // Quitar el nodo XAdES temporal insertado en la raíz del documento
            // (ya quedó embebido correctamente dentro del bloque <Signature>)
            
            
            //xmlDoc.DocumentElement!.RemoveChild(xadesNodoImportado);

            // Añadir el <Signature> completo al documento

            xmlDoc.DocumentElement!.AppendChild(xmlDoc.ImportNode(xmlFirma, true));

            using var stringWriter = new Utf8StringWriter();
            xmlDoc.Save(stringWriter);
            return stringWriter.ToString();
        }

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }
    }

    // ── SignedXmlConId ────────────────────────────────────────────────────────
    public class SignedXmlConId : SignedXml
    {
        public SignedXmlConId(XmlDocument document) : base(document) { }

        public override XmlElement? GetIdElement(XmlDocument document, string idValue)
        {
            var element = base.GetIdElement(document, idValue);
            if (element is not null) return element;

            var xpath = $"//*[@Id='{idValue}' or @id='{idValue}']";
            return document.SelectSingleNode(xpath) as XmlElement;
        }
    }

    // ── RsaPkcs1Sha256SignatureDescription ────────────────────────────────────
    public class RsaPkcs1Sha256SignatureDescription : SignatureDescription
    {
        public RsaPkcs1Sha256SignatureDescription()
        {
            KeyAlgorithm = typeof(RSA).FullName;
            DigestAlgorithm = typeof(SHA256).FullName;
            FormatterAlgorithm = typeof(RSAPKCS1SignatureFormatter).FullName;
            DeformatterAlgorithm = typeof(RSAPKCS1SignatureDeformatter).FullName;
        }

        public override AsymmetricSignatureFormatter CreateFormatter(AsymmetricAlgorithm key)
        {
            ArgumentNullException.ThrowIfNull(key);
            var formatter = new RSAPKCS1SignatureFormatter(key);
            formatter.SetHashAlgorithm("SHA256");
            return formatter;
        }

        public override AsymmetricSignatureDeformatter CreateDeformatter(AsymmetricAlgorithm key)
        {
            ArgumentNullException.ThrowIfNull(key);
            var deformatter = new RSAPKCS1SignatureDeformatter(key);
            deformatter.SetHashAlgorithm("SHA256");
            return deformatter;
        }
    }
}
