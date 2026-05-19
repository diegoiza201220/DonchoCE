// Services/SriService.cs
using ComprobantesElectronicos.DTO.Sri;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;

namespace ComprobantesElectronicos.Services;

public class SriService
{
    private readonly IConfiguration _config;
    private readonly bool _esProd;
    private HttpClient _httpClient;

    // URLs de los webservices
    private const string UrlRecepcionPruebas = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline";
    private const string UrlAutorizacionPruebas = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline";
    private const string UrlRecepcionProduccion = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline";
    private const string UrlAutorizacionProduccion = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline";

    public SriService(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _esProd = bool.Parse(config["Sri:Produccion"] ?? "false");
        _httpClient = httpClient;
    }

    // ── Paso 1: Enviar el XML firmado al SRI ─────────────────────────────────
    public async Task<RespuestaRecepcionSri> EnviarComprobanteAsync(string xmlBytesFirmadoBase64)
    {
        var url = _esProd ? UrlRecepcionProduccion : UrlRecepcionPruebas;
        var soapBody = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
            <soapenv:Envelope
                xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                xmlns:ec=""http://ec.gob.sri.ws.recepcion"">
                <soapenv:Header/>
                <soapenv:Body>
                    <ec:validarComprobante>
                        <xml>{xmlBytesFirmadoBase64}</xml>
                    </ec:validarComprobante>
                </soapenv:Body>
            </soapenv:Envelope>";

        var respuestaXml = await EnviarSoapAsync(url, soapBody, "validarComprobante");
        return ParsearRespuestaRecepcion(respuestaXml);
    }

    // ── Paso 2: Consultar autorización con la clave de acceso ────────────────
    public async Task<RespuestaAutorizacionSri> ConsultarAutorizacionAsync(string claveAcceso)
    {
        var url = _esProd ? UrlAutorizacionProduccion : UrlAutorizacionPruebas;
        var soapBody = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
            <soapenv:Envelope
                xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/""
                xmlns:ec=""http://ec.gob.sri.ws.autorizacion"">
                <soapenv:Header/>
                <soapenv:Body>
                    <ec:autorizacionComprobante>
                        <claveAccesoComprobante>{claveAcceso}</claveAccesoComprobante>
                    </ec:autorizacionComprobante>
                </soapenv:Body>
            </soapenv:Envelope>";

        var respuestaXml = await EnviarSoapAsync(url, soapBody, "autorizacionComprobante");
        return ParsearRespuestaAutorizacion(respuestaXml);
    }

    // ── Envío HTTP genérico ───────────────────────────────────────────────────
    private async Task<XDocument> EnviarSoapAsync(string url, string soapBody, string accion)
    {
        var content = new StringContent(soapBody, Encoding.UTF8, "text/xml");
        var handler = new HttpClientHandler { UseCookies = false };
        var client = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(3) };
        try
        {
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return XDocument.Parse(responseBody);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            // El servidor SOAP tardó demasiado tiempo en responder (Expiró el HttpClient.Timeout)
            Console.WriteLine("Error: La solicitud al servicio SOAP expiró por Timeout.");
        }
        catch (TaskCanceledException ex)
        {
            // La tarea fue cancelada a través de un CancellationToken provisto por código
            Console.WriteLine("Error: La operación fue cancelada por el usuario o el sistema.");
        }

 

        //return new XDocument();


        //try
        //{
        //    var response = await _httpClient.PostAsync(url, content);
        //    response.EnsureSuccessStatusCode();

        //    
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"{ex.Message}");
        //}
        return new XDocument();

    }

    // ── Parsear respuesta de recepción ────────────────────────────────────────
    private static RespuestaRecepcionSri ParsearRespuestaRecepcion(XDocument xml)
    {
        XNamespace ns = "http://ec.gob.sri.ws.recepcion";

        var estadoNodo = xml.Descendants(ns + "validarComprobanteResponse").FirstOrDefault();
        var mensajes = estadoNodo.Descendants("mensaje").Select(m => new MensajeSri
        {
            Identificador = m.Element("identificador")?.Value ?? "",
            Mensaje = m.Element("mensaje")?.Value ?? "",
            Tipo = m.Element("tipo")?.Value ?? "",
            Informacion = m.Element("informacionAdicional")?.Value ?? ""
        }).ToList();

        return new RespuestaRecepcionSri
        {
            Estado = estadoNodo?.Value ?? "ERROR",
            Mensajes = mensajes
        };
    }

    // ── Parsear respuesta de autorización ─────────────────────────────────────
    private static RespuestaAutorizacionSri ParsearRespuestaAutorizacion(XDocument xml)
    {
        var autorizacion = xml.Descendants("autorizacion").FirstOrDefault();
        var mensajes = autorizacion
            .Descendants("mensaje")
            .Select(m => new MensajeSri
            {
                Identificador = m.Element("identificador")?.Value ?? "",
                Mensaje = m.Element("mensaje")?.Value ?? "",
                Tipo = m.Element("tipo")?.Value ?? "",
                Informacion = m.Element("informacionAdicional")?.Value ?? ""
            }).ToList();

        DateTimeOffset.TryParse(
    autorizacion.Element("fechaAutorizacion")?.Value,
    out var fechaAutorizacion);

        return new RespuestaAutorizacionSri
        {
            Estado = autorizacion.Element("estado")?.Value ?? "",
            NumeroAutorizacion = autorizacion.Element("numeroAutorizacion")?.Value ?? "",
            FechaAutorizacion = fechaAutorizacion,
            Ambiente = autorizacion.Element("ambiente")?.Value ?? "",
            XmlAutorizado = autorizacion.Element("comprobante")?.Value ?? "",
            Mensajes = mensajes
        };

        //XNamespace ns = "http://ec.gob.sri.ws.autorizacion";

        //var autorizacion = xml.Descendants(ns + "autorizacionComprobanteResponse").FirstOrDefault();
        //if (autorizacion is null)
        //    return new RespuestaAutorizacionSri { Estado = "NO_AUTORIZADO" };

        //var mensajes = autorizacion.Descendants("mensaje").Select(m => new MensajeSri
        //{
        //    Identificador = m.Element("identificador")?.Value ?? "",
        //    Mensaje = m.Element("mensaje")?.Value ?? "",
        //    Tipo = m.Element("tipo")?.Value ?? "",
        //    Informacion = m.Element("informacionAdicional")?.Value ?? ""
        //}).ToList();


        //var estado = autorizacion.Value;//?.FirstNode().Value ?? "NO_AUTORIZADO";
        //var test = autorizacion.Value;


        //return new RespuestaAutorizacionSri
        //{
        //    Estado = autorizacion.Element(ns + "estado")?.Value ?? "NO_AUTORIZADO",
        //    NumeroAutorizacion = autorizacion.Element(ns + "numeroAutorizacion")?.Value ?? "",
        //    FechaAutorizacion = DateTime.TryParse(
        //                             autorizacion.Element(ns + "fechaAutorizacion")?.Value,
        //                             out var fecha) ? fecha : DateTime.MinValue,
        //    Ambiente = autorizacion.Element(ns + "ambiente")?.Value ?? "",
        //    //XmlAutorizado = autorizacion.Element(ns + "comprobante")?.Value ?? "",
        //    Mensajes = mensajes
        //};

        //return 
    }

    public async Task<bool> VerificarConectividadAsync()
    {
        try
        {
            var url = _esProd ? UrlRecepcionProduccion : UrlRecepcionPruebas;
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
            //_httpClient
            // Solo un GET para verificar que el servidor responde
            var response = await _httpClient.GetAsync(url + "?wsdl", cts.Token);
            Console.WriteLine($"SRI responde con status: {response.StatusCode}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Sin conectividad al SRI: {ex.Message}");
            return false;
        }
    }

    //private static BasicHttpsBinding CrearBinding() => new BasicHttpsBinding
    //{
    //    MaxReceivedMessageSize = 5 * 1024 * 1024, // 5 MB
    //    SendTimeout = TimeSpan.FromSeconds(30),
    //    ReceiveTimeout = TimeSpan.FromSeconds(30)
    //};
}