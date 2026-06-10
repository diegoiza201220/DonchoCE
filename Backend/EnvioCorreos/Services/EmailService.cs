using EFModel.DTO;
using EFModel.Interfaces;
using EFModel.Models;
using EnvioCorreos.Configuration;
using EnvioCorreos.Interfaces;
using EnvioCorreos.Models;
using FastReport;
using FastReport.Data;
using FastReport.Data.JsonConnection;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.Json;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace EnvioCorreos.Services
{
    public class EmailService : IEmailService
    {
        private readonly IUnitOfWork _uow;
        private readonly ICacheService _cache;
        private readonly EmailOptions _options;
        private readonly ILogger<EmailService> _logger;
        private const string JSON_SCHEMA_FACTURA = "JSON_SCHEMA_FACTURA";
        private const string PATH_LOCAL_FACTURAS = "PATH_LOCAL_FACTURAS";
        private readonly GenParametro genParametroJsonSchemaFactura;
        private readonly GenParametro genParametroPathLocalFacturas;
        private readonly IConfiguration _config;

        public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger, ICacheService cache, IUnitOfWork uow, IConfiguration config)
        {
            _options = options.Value;
            _logger = logger;
            _cache = cache;
            _uow = uow;
            _config = config;
            genParametroJsonSchemaFactura = _cache.GetOrCreatePermanent(JSON_SCHEMA_FACTURA, () => _uow.GenParametroR.GetById(JSON_SCHEMA_FACTURA));
            genParametroPathLocalFacturas = _cache.GetOrCreatePermanent(PATH_LOCAL_FACTURAS, () => _uow.GenParametroR.GetById(PATH_LOCAL_FACTURAS));
        }

        public MemoryStream ImprimirPDF(FacOrdenDTO orden)
        {


            using (Report report = new())
            {
                report.Load("reportes/rptFactura.frx");
                var json = JsonConvert.SerializeObject(orden, Formatting.None);
                string jsonModificado = $"Json='{json}'{genParametroJsonSchemaFactura.Valor}";
                foreach (var connection in report.Dictionary.Connections)
                {
                    if (connection is JsonDataSourceConnection jsonConnection)
                    {
                        try
                        {
                            jsonConnection.ConnectionString = jsonModificado;
                            jsonConnection.CreateAllTables();
                        }
                        finally
                        {
                            // Limpiar el archivo temporal siempre, aunque falle
                        }
                    }
                }

                report.Prepare();

                using var pdfStream = new MemoryStream();
                var filename = $"{genParametroPathLocalFacturas.Valor}{orden.ClaveNumeroAutorizacion}.pdf";
                using (MemoryStream ms = new MemoryStream())
                {
                    PDFSimpleExport pdfExport = new PDFSimpleExport();
                    report.Export(pdfExport, ms);
                    //File.WriteAllBytes(filename, ms.ToArray());
                    return ms;
                }

            }
        }
        public async Task<ResultadoEmail> EnviarAsync(FacOrdenDTO orden)
        {
            try
            {
                MemoryStream pdfFactura = ImprimirPDF(orden);

                var mensaje = new EmailMessage
                {
                    Destinatarios = new List<string> { orden.Cliente.Email ?? _config["Email:usuario"] },
                    Asunto = $"Doncho - Factura Electrónica {orden.Establecimiento}-{orden.PuntoEmision}-{orden.NumeroFactura}",
                    EsHtml = true,
                    Cuerpo = PlantillaFactura(orden.Cliente.Nombre, $"{orden.Establecimiento}-{orden.PuntoEmision}-{orden.NumeroFactura}"),
                    Adjuntos =
                            [
                                EmailAdjunto.DesdePdf($"Factura_{orden.Establecimiento}-{orden.PuntoEmision}-{orden.NumeroFactura}.pdf", pdfFactura.ToArray()),
                                EmailAdjunto.DesdeXml($"Factura_{orden.Establecimiento}-{orden.PuntoEmision}-{orden.NumeroFactura}.xml", orden.Xml)
                            ]
                };

                var mimeMessage = ConstruirMimeMessage(mensaje);
                _ = EnviarSmtpAsync(mimeMessage);

                _logger.LogInformation(
                    "Email enviado a {Destinatarios} — Asunto: {Asunto}",
                    string.Join(", ", mensaje.Destinatarios),
                    mensaje.Asunto);

                return ResultadoEmail.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando email a {Destinatarios}",
                    string.Join(", ", $"{orden.Cliente.Nombre} {orden.Cliente.Apellido}-{orden.Cliente.Email}"));
                return ResultadoEmail.Fallo(ex.Message);
            }
        }

        // Método especializado para envío de facturas electrónicas
        public async Task<ResultadoEmail> EnviarFacturaAsync(
            string destinatario,
            string nombreCliente,
            string numeroFactura,
            byte[] ridePdf,
            string xmlFirmado, FacOrdenDTO facOrdenDTO)
        {
            var mensaje = new EmailMessage
            {
                Destinatarios = [destinatario],
                Asunto = $"Factura Electrónica {numeroFactura}",
                EsHtml = true,
                Cuerpo = PlantillaFactura(nombreCliente, numeroFactura),
                Adjuntos =
                [
                    EmailAdjunto.DesdePdf($"Factura_{numeroFactura}.pdf", ridePdf),
                EmailAdjunto.DesdeXml($"Factura_{numeroFactura}.xml", xmlFirmado)
                ]
            };

            return await EnviarAsync(facOrdenDTO);
        }

        private MimeMessage ConstruirMimeMessage(EmailMessage mensaje)
        {
            var mimeMessage = new MimeMessage();

            // Remitente
            mimeMessage.From.Add(new MailboxAddress(
                _options.NombreRemitente,
                _options.Usuario));

            // Destinatarios
            foreach (var destinatario in mensaje.Destinatarios)
                mimeMessage.To.Add(MailboxAddress.Parse(destinatario));

            // CC y BCC
            foreach (var cc in mensaje.ConCopia)
                mimeMessage.Cc.Add(MailboxAddress.Parse(cc));

            foreach (var bcc in mensaje.ConCopiaOculta)
                mimeMessage.Bcc.Add(MailboxAddress.Parse(bcc));

            mimeMessage.Subject = mensaje.Asunto;

            // Construir el cuerpo con o sin adjuntos
            var builder = new BodyBuilder();

            if (mensaje.EsHtml)
                builder.HtmlBody = mensaje.Cuerpo;
            else
                builder.TextBody = mensaje.Cuerpo;

            foreach (var adjunto in mensaje.Adjuntos)
                builder.Attachments.Add(
                    adjunto.NombreArchivo,
                    adjunto.Contenido,
                    ContentType.Parse(adjunto.TipoMime));

            mimeMessage.Body = builder.ToMessageBody();
            return mimeMessage;
        }

        private async Task EnviarSmtpAsync(MimeMessage mimeMessage)
        {
            using var smtp = new SmtpClient();

            var secureSocket = _options.UsarSsl
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.None;

            await smtp.ConnectAsync(_options.Host, _options.Port, secureSocket);
            await smtp.AuthenticateAsync(_options.Usuario, _options.Password);
            await smtp.SendAsync(mimeMessage);
            await smtp.DisconnectAsync(true);
        }

        private static string PlantillaFactura(string nombreCliente, string numeroFactura) => $"""
        <html>
        <body style="font-family: Arial, sans-serif; color: #333;">
            <h2>Estimado/a {nombreCliente},</h2>
            <p>Adjunto encontrará su factura electrónica <strong>{numeroFactura}</strong>
               autorizada por el SRI.</p>
            <ul>
                <li><strong>PDF:</strong> Representación impresa del documento (RIDE)</li>
                <li><strong>XML:</strong> Documento electrónico autorizado</li>
            </ul>
            <p style="color: #666; font-size: 12px;">
                Este es un mensaje automático, por favor no responda este correo.
            </p>
        </body>
        </html>
        """;
    }
}
