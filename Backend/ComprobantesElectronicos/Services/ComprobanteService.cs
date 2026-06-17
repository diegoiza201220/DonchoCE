using ComprobantesElectronicos.DTO.Sri;
using ComprobantesElectronicos.Utils;
using EFModel.DTO;
using EFModel.Interfaces;
using EFModel.Models;
using EnvioCorreos.Services;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace ComprobantesElectronicos.Services;

public class ComprobanteService(SriService sriService, InfowareFirmaService infowareFirmaService, IUnitOfWork uow, EmailService emailService, IConfiguration config)
{
    public async Task<ResultadoEmisionDTO> EmitirFacturaAsync(FacOrdenDTO ordenDTO, CelLogDocumento celLogDocumento)
    {
        //Task.Run(async () =>
        //{
        try
        {
            Console.WriteLine("emitir factura");
            // 1. Generar el XML del comprobante
            var entidadSri = ConvertirAEntidadSri.ObtenerFactura(ordenDTO);

            // 2. Firmar el XML con XAdES-BES
            var xmlFirmado = infowareFirmaService.FirmarDocumento(entidadSri);

            // 3. Convertir a Base64 para enviar al SRI
            var xmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlFirmado));

            // 4. Enviar al SRI
            var respuestaRecepcion = await sriService.EnviarComprobanteAsync(xmlBase64);

            if (!respuestaRecepcion.FueRecibida)
            {
                SetInformacionCelLogDocumento(celLogDocumento, estado: 1, string.Join("; ", respuestaRecepcion.Mensajes.Select(m => m.Mensaje)));// estado 1 error en recepción
                await uow.SaveChangesAsync();
                return new ResultadoEmisionDTO
                {
                    Exitoso = false,
                    Mensajes = [.. respuestaRecepcion.Mensajes.Select(m => m.Mensaje)]
                };
            }

            // 5. Esperar y consultar autorización (el SRI puede tardar unos segundos)
            await Task.Delay(2000);
            var claveAcceso = ordenDTO.ClaveNumeroAutorizacion;
            var respuestaAutorizacion = await sriService.ConsultarAutorizacionAsync(claveAcceso);

            if (!respuestaAutorizacion.FueAutorizado)
            {
                SetInformacionCelLogDocumento(celLogDocumento, estado: 2, string.Join("; ", respuestaRecepcion.Mensajes.Select(m => m.Mensaje)));// estado 2 error en autorización
                await uow.SaveChangesAsync();
                return new ResultadoEmisionDTO
                {
                    Exitoso = false,
                    Mensajes = [.. respuestaAutorizacion.Mensajes.Select(m => m.Mensaje)]
                };
            }

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
