using EFModel.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.IsisMtt.Ocsp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.X509;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;

namespace ComprobantesElectronicos.Services
{

    public class FirmaElectronicaService
    {
        private readonly IConfiguration _config;

        public FirmaElectronicaService(IConfiguration config) => _config = config;

        private X509Certificate2 CargarCertificado()
        {
            try
            {
                var rutaCertificado = _config["FirmaElectronica:RutaCertificado"];
                var clave = _config["FirmaElectronica:Clave"];
                X509Certificate2 cert = new X509Certificate2(rutaCertificado, clave, X509KeyStorageFlags.MachineKeySet);

                //var cert = X509CertificateLoader.LoadCertificateFromFile(rutaCertificado);
                return cert;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error-> {ex.Message}");
                throw;
            }

            return null;
        }

        private string FirmarXml(string xmlSinFirmar)
        {
            var certificado = CargarCertificado();
            //try
            //{
            //    certificado = CargarCertificado();
            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine($"error-> {ex.Message}");
            //}


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

        public string GenerarXMLFacturaFirmado(FacOrden orden)
        {
            factura factura = new()
            {
                id = "comprobante",
                version = "1.0",
                infoTributaria = new facturaInfoTributaria()
                {
                    ambiente = 1, // 1 para pruebas, 2 para producción
                    tipoEmision = 1, // 1 para emisión normal
                    razonSocial = "juan cuenca",
                    nombreComercial = "JUAN CUENCA",
                    ruc = "0993069000001",
                    claveAcceso = "123456",
                    codDoc = "01", // Código para factura
                    contribuyenteRimpe = "SI",
                    dirMatriz = "CENTRO HISTÓRICO QUITO",
                    estab = "001",
                    ptoEmi = "001",
                    secuencial = orden.Secuencial.ToString("D9") // Formatear a 9 dígitos
                },
                infoFactura = new facturaInfoFactura()
                {
                    fechaEmision = orden.Fecha.ToString("dd/mm/aaaa"),
                    dirEstablecimiento = "CENTRO HISTORICO",
                    obligadoContabilidad = "NO",
                    tipoIdentificacionComprador = "05",
                    razonSocialComprador = $"{orden.Cliente.Apellido} {orden.Cliente.Nombre}",
                    identificacionComprador = orden.Cliente.CedulaRuc,
                    direccionComprador = orden.Cliente.Direccion ?? string.Empty,
                    totalSinImpuestos = orden.TotalSinImpuestos,
                    totalDescuento = 0,
                    propina = 0,
                    importeTotal = orden.TotalOrden,
                    moneda = "DOLAR",
                }
            };
            factura.infoFactura.totalConImpuestos = new facturaInfoFacturaTotalImpuesto[1];
            facturaInfoFacturaTotalImpuesto facturaInfoFacturaTotalImpuesto = new()
            {
                codigo = orden.ImpuestoCodigo,
                codigoPorcentaje = orden.ImpuestoCodigoPorcentaje,
                descuentoAdicional = 0,
                baseImponible = orden.ImpuestoBaseImponible,
                valor = orden.ImpuestoValor,
            };
            factura.infoFactura.totalConImpuestos[0] = facturaInfoFacturaTotalImpuesto;
            factura.infoFactura.pagos = new facturaInfoFacturaPago[1];
            facturaInfoFacturaPago facturaInfoFacturaPago = new()
            {
                formaPago = 1,
                total = orden.TotalOrden,
                plazo = 0,
                unidadTiempo = "dias"
            };
            factura.infoFactura.pagos[0] = facturaInfoFacturaPago;
            factura.detalles = new facturaDetalle[orden.FacDetalleOrdens.Count];

            List<facturaDetalle> lfdetalle = [];
            foreach (var item in orden.FacDetalleOrdens)
            {
                facturaDetalle facturaDetalle = new()
                {
                    codigoPrincipal = item.Productoid.ToString(),
                    descripcion = item.Producto?.Nombre ?? string.Empty,
                    cantidad = item.Cantidad,
                    precioUnitario = item.PrecioUnitario,
                    descuento = 0,
                    precioTotalSinImpuesto = item.PrecioTotal,
                    impuestos = new facturaDetalleImpuesto[1]
                };
                facturaDetalleImpuesto facturaDetalleImpuesto = new()
                {
                    codigo = 2,
                    codigoPorcentaje = 4,
                    tarifa = 15,
                    baseImponible = item.PrecioTotal,
                    valor = item.ValorIva
                };
                facturaDetalle.impuestos[0] = facturaDetalleImpuesto;
                lfdetalle.Add(facturaDetalle);
            }

            var xmlFirmado = FirmarXml(factura.ToString());

            //SriService sriService = new(_config);
            return xmlFirmado;
            // Aquí iría la lógica para generar el XML de la factura
            // Esto podría incluir la creación de un XmlDocument, agregar los elementos necesarios,
            // y luego devolver el XML como string para ser firmado.
        }


    }
}
