using ComprobantesElectronicos.DTO.Sri;
using ComprobantesElectronicos.Utils;
using EFModel.DTO;
using EFModel.Interfaces;
using EFModel.Models;
using EnvioCorreos.Models;
using EnvioCorreos.Services;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;
using System.Text;

namespace ComprobantesElectronicos.Services;

public class ComprobanteService
{
    private readonly SriService _sriService;
    private readonly InfowareFirmaService _infowareFirmaService;
    private readonly IUnitOfWork _uow;
    private readonly EmailService _emailService;


    public ComprobanteService(SriService sriService, InfowareFirmaService infowareFirmaService, IUnitOfWork uow, EmailService emailService, IConfiguration config)
    {
        _sriService = sriService;
        _infowareFirmaService = infowareFirmaService;
        _uow = uow;
        _emailService = emailService;
    }
    public async Task<ResultadoEmisionDTO> EmitirFacturaAsync(FacOrdenDTO ordenDTO, CelLogDocumento celLogDocumento)
    {
        //Task.Run(async () =>
        //{
        try
        {
            Console.WriteLine("a emitir factura");
            //await Task.Delay(60000);
            Console.WriteLine("ahora si a emitir factura");
            // 1. Generar el XML del comprobante
            var entidadSri = ConvertirAEntidadSri.ObtenerFactura(ordenDTO);

            // 2. Firmar el XML con XAdES-BES
            var xmlFirmado = _infowareFirmaService.FirmarDocumento(entidadSri);

            // 3. Convertir a Base64 para enviar al SRI
            var xmlBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xmlFirmado));

            // 4. Enviar al SRI
            var respuestaRecepcion = await _sriService.EnviarComprobanteAsync(xmlBase64);

            if (!respuestaRecepcion.FueRecibida)
            {
                SetInformacionCelLogDocumento(celLogDocumento,estado: 1, string.Join("; ", respuestaRecepcion.Mensajes.Select(m => m.Mensaje)));// estado 1 error en recepción
                return new ResultadoEmisionDTO
                {
                    Exitoso = false,
                    Mensajes = respuestaRecepcion.Mensajes.Select(m => m.Mensaje).ToList()
                };
            }

            // 5. Esperar y consultar autorización (el SRI puede tardar unos segundos)
            await Task.Delay(3000);
            var claveAcceso = ordenDTO.ClaveNumeroAutorizacion;
            var respuestaAutorizacion = await _sriService.ConsultarAutorizacionAsync(claveAcceso);

            if (!respuestaAutorizacion.FueAutorizado)
            {
                SetInformacionCelLogDocumento(celLogDocumento, estado: 2, string.Join("; ", respuestaRecepcion.Mensajes.Select(m => m.Mensaje)));// estado 2 error en autorización
                return new ResultadoEmisionDTO
                {
                    Exitoso = false,
                    Mensajes = respuestaAutorizacion.Mensajes.Select(m => m.Mensaje).ToList()
                };
            }

            celLogDocumento.XmlFirmado = ordenDTO.Xml = respuestaAutorizacion.XmlAutorizado;
            SetInformacionCelLogDocumento(celLogDocumento, estado: 200, mensaje: "Comprobante autorizado exitosamente");

            await _uow.SaveChangesAsync();

            //EmailMessage emailMessage = new()
            //{
            //    Asunto = "Factura de su compra",
            //    Cuerpo = $"Estimado {ordenDTO.Cliente.Nombre}, adjunto encontrará la factura de su compra. Gracias por elegirnos.",
            //    Destinatarios = new List<string> { ordenDTO.Cliente.Email ?? _config["Email:usuario"] },
            //    EsHtml = false
            //};


            _ = _emailService.EnviarAsync(ordenDTO);

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
                Mensajes = new List<string> { $"Error al emitir comprobante: {ex.Message}" }
            };
        }
    }

    public void SetInformacionCelLogDocumento(CelLogDocumento celLogDocumento, int estado, string mensaje)
    {
        celLogDocumento.Estado = estado;
        celLogDocumento.Mensaje = mensaje;
        _uow.CelLogDocumentoR.Update(celLogDocumento);
    }
}
