using ComprobantesElectronicos.DTO.Sri;
using ComprobantesElectronicos.Utils;
using EFModel.DTO;
using EFModel.Interfaces;
using EFModel.Models;
using EnvioCorreos.Services;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ComprobantesElectronicos.Services;

public class ComprobanteService(SriService sriService, InfowareFirmaService infowareFirmaService, IUnitOfWork uow, EmailService emailService, ILogger<ComprobanteService> logger)
{
    public async Task<ResultadoEmisionDTO> EmitirFacturaAsync(FacOrdenDTO ordenDTO, CelLogDocumento celLogDocumento)
    {
        try
        {
            logger.LogInformation($"IdFromFe:{ordenDTO.IdFromFe} - EmitirFactura");
            // 1. Generar el XML del comprobante
            var entidadSri = ConvertirAEntidadSri.ObtenerFactura(ordenDTO);
            logger.LogInformation($"IdFromFe:{ordenDTO.IdFromFe} - formato factura SRI - OK");

            // 2. Firmar el XML con XAdES-BES
            var xmlFirmado = infowareFirmaService.FirmarDocumento(entidadSri);
            logger.LogInformation($"IdFromFe:{ordenDTO.IdFromFe} - firma de documento - OK");

            // 3. Convertir a Base64 para enviar al SRI
            var xmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlFirmado));
            logger.LogInformation($"IdFromFe:{ordenDTO.IdFromFe} - conversión a Base64 - OK");

            // 4. Enviar al SRI
            var respuestaRecepcion = await sriService.EnviarComprobanteAsync(xmlBase64);
            logger.LogInformation($"IdFromFe:{ordenDTO.IdFromFe} - enviarcomprobante - OK");

            if (!respuestaRecepcion.FueRecibida)
            {
                celLogDocumento.XmlFirmado = xmlFirmado;
                SetInformacionCelLogDocumento(celLogDocumento, estado: 1, string.Join("; ", respuestaRecepcion.Mensajes.Select(m => m.Informacion)));// estado 1 error en recepción
                await uow.SaveChangesAsync();
                return new ResultadoEmisionDTO
                {
                    Exitoso = false,
                    Mensajes = [.. respuestaRecepcion.Mensajes.Select(m => m.Mensaje)]
                };
            }
            logger.LogInformation($"IdFromFe:{ordenDTO.IdFromFe} - respuestarecepción - OK");

            // 5. Esperar y consultar autorización (el SRI puede tardar unos segundos)
            await Task.Delay(2000);
            var claveAcceso = ordenDTO.ClaveNumeroAutorizacion;
            var respuestaAutorizacion = await sriService.ConsultarAutorizacionAsync(claveAcceso);

            if (!respuestaAutorizacion.FueAutorizado)
            {
                celLogDocumento.XmlFirmado = xmlFirmado;
                SetInformacionCelLogDocumento(celLogDocumento, estado: 2, string.Join("; ", respuestaRecepcion.Mensajes.Select(m => m.Informacion)));// estado 2 error en autorización
                await uow.SaveChangesAsync();
                return new ResultadoEmisionDTO
                {
                    Exitoso = false,
                    Mensajes = [.. respuestaAutorizacion.Mensajes.Select(m => m.Mensaje)]
                };
            }
            logger.LogInformation($"IdFromFe:{ordenDTO.IdFromFe} - respuestaautorización - OK");

            celLogDocumento.XmlFirmado = ordenDTO.Xml = respuestaAutorizacion.XmlAutorizado;
            SetInformacionCelLogDocumento(celLogDocumento, estado: 200, mensaje: "Comprobante autorizado exitosamente");

            await uow.SaveChangesAsync();

            _ = emailService.EnviarAsync(ordenDTO);

            return new ResultadoEmisionDTO
            {
                Exitoso = true,
                NumeroAutorizacion = respuestaAutorizacion.NumeroAutorizacion,
                FechaAutorizacion = respuestaAutorizacion.FechaAutorizacion,
                XmlAutorizado = respuestaAutorizacion.XmlAutorizado
            };
        }
        catch (Exception ex)
        {
            SetInformacionCelLogDocumento(celLogDocumento, estado: 3, mensaje: $"Error al emitir comprobante: {ex.Message}");// estado 3 error en autorización)
            await uow.SaveChangesAsync();
            logger.LogError($"IdFromFe:{ordenDTO.IdFromFe} - mensaje: {ex.Message}");
            return new ResultadoEmisionDTO
            {
                Exitoso = false,
                Mensajes = [$"Error al emitir comprobante: {ex.Message}"]
            };
        }
    }

    public void SetInformacionCelLogDocumento(CelLogDocumento celLogDocumento, int estado, string mensaje)
    {
        celLogDocumento.Estado = estado;
        celLogDocumento.Mensaje = mensaje;
        uow.CelLogDocumentoR.Update(celLogDocumento);
    }
}
