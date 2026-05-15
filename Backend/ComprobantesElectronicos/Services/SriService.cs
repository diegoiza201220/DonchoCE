// Services/SriService.cs
using ComprobantesElectronicos.DTO.Sri;
using Microsoft.Extensions.Configuration;
using ServiceAutorizacionComprobantes;
using ServiceRecepcionComprobantes;
using System.ServiceModel;
using mensaje = ServiceRecepcionComprobantes.mensaje;

namespace ComprobantesElectronicos.Services;

public class SriService
{
    private readonly IConfiguration _config;
    private readonly bool _esProd;

    // URLs de los webservices
    private const string UrlRecepcionPruebas = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline";
    private const string UrlAutorizacionPruebas = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline";
    private const string UrlRecepcionProduccion = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline";
    private const string UrlAutorizacionProduccion = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline";

    public SriService(IConfiguration config)
    {
        _config = config;
        _esProd = bool.Parse(config["Sri:Produccion"] ?? "false");
    }

    // ── Paso 1: Enviar el XML firmado al SRI ─────────────────────────────────
    public async Task<RespuestaRecepcionSri> EnviarComprobanteAsync(byte[] xmlBytesFirmadoBase64)
    {
        var url = _esProd ? UrlRecepcionProduccion : UrlRecepcionPruebas;
        var binding = CrearBinding();
        var cliente = new RecepcionComprobantesOfflineClient(binding, new EndpointAddress(url));

        try
        {
            //var xmlBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(xmlFirmadoBase64));

            var respuesta = await cliente.validarComprobanteAsync(xmlBytesFirmadoBase64);

            return new RespuestaRecepcionSri
            {
                Estado = respuesta.RespuestaRecepcionComprobante.estado,
                Mensajes = respuesta.RespuestaRecepcionComprobante.comprobantes?
                    .SelectMany(c => c.mensajes ?? Array.Empty<mensaje>())
                    .Select(m => new MensajeSri
                    {
                        Identificador = m.identificador,
                        Mensaje = m.mensaje1,
                        Tipo = m.tipo,
                        Informacion = m.informacionAdicional
                    }).ToList() ?? new()
            };
        }
        finally
        {
            await cliente.CloseAsync();
        }
    }

    // ── Paso 2: Consultar autorización con la clave de acceso ────────────────
    public async Task<RespuestaAutorizacionSri> ConsultarAutorizacionAsync(string claveAcceso)
    {
        var url = _esProd ? UrlAutorizacionProduccion : UrlAutorizacionPruebas;
        var binding = CrearBinding();
        var cliente = new AutorizacionComprobantesOfflineClient(binding, new EndpointAddress(url));

        try
        {
            var respuesta = await cliente.autorizacionComprobanteAsync(claveAcceso);
            var autorizacion = respuesta.RespuestaAutorizacionComprobante
                                        .autorizaciones?
                                        .FirstOrDefault();

            return new RespuestaAutorizacionSri
            {
                Estado = autorizacion?.estado ?? "NO_AUTORIZADO",
                NumeroAutorizacion = autorizacion?.numeroAutorizacion ?? "",
                FechaAutorizacion = autorizacion?.fechaAutorizacion ?? DateTime.MinValue,
                Ambiente = autorizacion?.ambiente ?? "",
                XmlAutorizado = autorizacion?.comprobante ?? "",
                Mensajes = autorizacion?.mensajes?
                    .Select(m => new MensajeSri
                    {
                        Identificador = m.identificador,
                        Mensaje = m.mensaje1,
                        Tipo = m.tipo,
                        Informacion = m.informacionAdicional
                    }).ToList() ?? new()
            };
        }
        finally
        {
            await cliente.CloseAsync();
        }
    }

    private static BasicHttpsBinding CrearBinding() => new BasicHttpsBinding
    {
        MaxReceivedMessageSize = 5 * 1024 * 1024, // 5 MB
        SendTimeout = TimeSpan.FromSeconds(30),
        ReceiveTimeout = TimeSpan.FromSeconds(30)
    };
}