using EnvioCorreos.Configuration;
using EnvioCorreos.Interfaces;
using EnvioCorreos.Models;
using FastReport;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using FastReport.Export.PdfSimple;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace EnvioCorreos.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _options;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public async Task<ResultadoEmail> EnviarAsync(EmailMessage mensaje)
        {
            try
            {
                using (Report report = new Report())
                {
                    // Aquí podrías cargar un reporte FastReport y exportarlo a PDF
                    // para adjuntarlo al correo. Este es solo un ejemplo básico.
                    report.Load("reportes/test.frx");
                    report.Prepare();

                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //    PDFSimpleExport pDFSimpleExport = new PDFSimpleExport();
                    //}

                    using var pdfStream = new MemoryStream();
                    
                    var filename = "c:\\diza\\Proyectos\\Reporte.pdf";
                    //report.Export(export, filename);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        PDFSimpleExport pdfExport = new PDFSimpleExport();
                        report.Export(pdfExport, ms);
                        File.WriteAllBytes(filename, ms.ToArray());
                    }


                    //pdfStream.Position = 0;
                    //mensaje.Adjuntos.Add(EmailAdjunto.DesdePdf("Reporte.pdf", pdfStream.ToArray()));
                }

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
                    string.Join(", ", mensaje.Destinatarios));
                return ResultadoEmail.Fallo(ex.Message);
            }
        }

        // Método especializado para envío de facturas electrónicas
        public async Task<ResultadoEmail> EnviarFacturaAsync(
            string destinatario,
            string nombreCliente,
            string numeroFactura,
            byte[] ridePdf,
            string xmlFirmado)
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

            return await EnviarAsync(mensaje);
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
