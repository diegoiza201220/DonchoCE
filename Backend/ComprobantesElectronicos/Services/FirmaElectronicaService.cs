using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using Microsoft.Extensions.Configuration;
using EFModel.Models;

namespace ComprobantesElectronicos.Services
{

    public class FirmaElectronicaService
    {
        private readonly IConfiguration _config;

        public FirmaElectronicaService(IConfiguration config) => _config = config;

        private X509Certificate2 CargarCertificado()
        {
            var rutaCertificado = _config["FirmaElectronica:RutaCertificado"];
            var clave = _config["FirmaElectronica:Clave"];

            return new X509Certificate2(
                rutaCertificado!,
                clave,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet
            );
        }

        public string FirmarXml(string xmlSinFirmar)
        {
            var certificado = CargarCertificado();
            var xmlDoc = new XmlDocument { PreserveWhitespace = true };
            xmlDoc.LoadXml(xmlSinFirmar);

            var elementoRaiz = xmlDoc.DocumentElement!;
            var idDocumento = "comprobante";
            elementoRaiz.SetAttribute("Id", idDocumento);

            var xadesNs = "http://uri.etsi.org/01903/v1.3.2#";
            var signatureId = $"Signature-{Guid.NewGuid():N}";
            var signedPropsId = $"SignedProperties-{signatureId}";

            var certHashB64 = Convert.ToBase64String(certificado.GetCertHash());
            var certIssuer = certificado.Issuer;
            var certSerial = certificado.SerialNumber;

            var xadesXml = $@"
        <xades:QualifyingProperties xmlns:xades=""{xadesNs}"" Target=""#{signatureId}"">
          <xades:SignedProperties Id=""{signedPropsId}"">
            <xades:SignedSignatureProperties>
              <xades:SigningTime>{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}</xades:SigningTime>
              <xades:SigningCertificate>
                <xades:Cert>
                  <xades:CertDigest>
                    <ds:DigestMethod xmlns:ds=""http://www.w3.org/2000/09/xmldsig#""
                                     Algorithm=""http://www.w3.org/2000/09/xmldsig#sha1""/>
                    <ds:DigestValue xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"">{certHashB64}</ds:DigestValue>
                  </xades:CertDigest>
                  <xades:IssuerSerial>
                    <ds:X509IssuerName xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"">{certIssuer}</ds:X509IssuerName>
                    <ds:X509SerialNumber xmlns:ds=""http://www.w3.org/2000/09/xmldsig#"">{certSerial}</ds:X509SerialNumber>
                  </xades:IssuerSerial>
                </xades:Cert>
              </xades:SigningCertificate>
            </xades:SignedSignatureProperties>
          </xades:SignedProperties>
        </xades:QualifyingProperties>";

            var xadesDoc = new XmlDocument();
            xadesDoc.LoadXml(xadesXml);

            var signedXml = new SignedXml(xmlDoc)
            {
                SigningKey = certificado.GetRSAPrivateKey()
            };
            signedXml.Signature.Id = signatureId;

            var refDocumento = new Reference($"#{idDocumento}")
            {
                DigestMethod = SignedXml.XmlDsigSHA1Url
            };
            refDocumento.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            signedXml.AddReference(refDocumento);

            var refXades = new Reference($"#{signedPropsId}")
            {
                Type = "http://uri.etsi.org/01903#SignedProperties",
                DigestMethod = SignedXml.XmlDsigSHA1Url
            };
            signedXml.AddReference(refXades);

            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(certificado));
            signedXml.KeyInfo = keyInfo;

            var dataObject = new DataObject();
            dataObject.Data = xadesDoc.ChildNodes;
            signedXml.AddObject(dataObject);

            signedXml.ComputeSignature();

            var xmlFirma = signedXml.GetXml();
            xmlDoc.DocumentElement!.AppendChild(xmlDoc.ImportNode(xmlFirma, true));

            return xmlDoc.OuterXml;
        }

        public string GenerarXMLFactura(FacOrden orden)
        {
            factura factura = new()
            {
                version = "1.0",
                
            };

            facturaInfoFactura infoFactura = new()
            {
                
            }
            


            factura.infoFactura.fechaEmision = orden.Fecha.ToString("yyyy-MM-dd");
            factura.infoFactura.tipoIdentificacionComprador

            return "";
            // Aquí iría la lógica para generar el XML de la factura
            // Esto podría incluir la creación de un XmlDocument, agregar los elementos necesarios,
            // y luego devolver el XML como string para ser firmado.
        }
    }
}
