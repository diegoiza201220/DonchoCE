using ComprobantesElectronicos.DTO.Sri;
using ComprobantesElectronicos.Utils;
using EFModel.DTO;
using EFModel.Interfaces;
using EFModel.Models;
using EnvioCorreos.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;

namespace ComprobantesElectronicos.Services;

public class ComprobanteService(SriService sriService, InfowareFirmaService infowareFirmaService, IUnitOfWork uow, EmailService emailService, IConfiguration config, ILogger<ComprobanteService> logger)
{
    public async Task<ResultadoEmisionDTO> EmitirFacturaAsync(FacOrdenDTO ordenDTO, CelLogDocumento celLogDocumento)
    {
        //Task.Run(async () =>
        //{
        try
        {
            // 1. Generar el XML del comprobante
            var entidadSri = ConvertirAEntidadSri.ObtenerFactura(ordenDTO);
            logger.LogInformation($"Proceso OK de obtención de factura para orden con Fecha:{ordenDTO.FechaInteger} - Secuencial:{ordenDTO.Secuencial} - Sucursal:{ordenDTO.Sucursalid} - Id: {ordenDTO.Id}");

            // 2. Firmar el XML con XAdES-BES
            var xmlFirmado = infowareFirmaService.FirmarDocumento(entidadSri);
            logger.LogInformation($"Proceso OK de firma de documento para orden con Fecha:{ordenDTO.FechaInteger} - Secuencial:{ordenDTO.Secuencial} - Sucursal:{ordenDTO.Sucursalid} - Id: {ordenDTO.Id}");

            // 3. Convertir a Base64 para enviar al SRI
            var xmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlFirmado));
            logger.LogInformation($"Proceso OK de conversión a Base64 para orden con Fecha:{ordenDTO.FechaInteger} - Secuencial:{ordenDTO.Secuencial} - Sucursal:{ordenDTO.Sucursalid} - Id: {ordenDTO.Id}");


            // 4. Enviar al SRI
            var respuestaRecepcion = await sriService.EnviarComprobanteAsync(xmlBase64);
            logger.LogInformation($"Proceso OK de envío de comprobante para orden con Fecha:{ordenDTO.FechaInteger} - Secuencial:{ordenDTO.Secuencial} - Sucursal:{ordenDTO.Sucursalid} - Id: {ordenDTO.Id}");

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
            logger.LogInformation($"Proceso OK de respuesta-recepción para orden con Fecha:{ordenDTO.FechaInteger} - Secuencial:{ordenDTO.Secuencial} - Sucursal:{ordenDTO.Sucursalid} - Id: {ordenDTO.Id}");

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
            logger.LogInformation($"Proceso OK de respuesta-autorización para orden con Fecha:{ordenDTO.FechaInteger} - Secuencial:{ordenDTO.Secuencial} - Sucursal:{ordenDTO.Sucursalid} - Id: {ordenDTO.Id}");

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
            logger.LogError($"Error en proceso de generación de factura para orden con Fecha:{ordenDTO.FechaInteger} - Secuencial:{ordenDTO.Secuencial} - Sucursal:{ordenDTO.Sucursalid} - Id: {ordenDTO.Id} - Ex: {ex.Message}");
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
