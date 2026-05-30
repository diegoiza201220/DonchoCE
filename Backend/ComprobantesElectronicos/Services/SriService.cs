using ComprobantesElectronicos.DTO.Sri;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Xml.Linq;

namespace ComprobantesElectronicos.Services;

public class SriService
{
    private readonly bool _esProd;
    private HttpClient _httpClient;

    // URLs de los webservices
    private const string UrlRecepcionPruebas = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline";
    private const string UrlAutorizacionPruebas = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline";
    private const string UrlRecepcionProduccion = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/RecepcionComprobantesOffline";
    private const string UrlAutorizacionProduccion = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline";

    public SriService(IConfiguration config, HttpClient httpClient)
    {
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
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3));
        try
        {
            var response = await _httpClient.PostAsync(url, content, cts.Token);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync(cts.Token);
            return XDocument.Parse(body);
        }
        catch (TaskCanceledException ex)
        {
            Console.WriteLine($"Timeout SRI: {ex.Message}");
            return new XDocument();
        }
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

        DateTimeOffset.TryParse(autorizacion.Element("fechaAutorizacion")?.Value,out var fechaAutorizacion);

        return new RespuestaAutorizacionSri
        {
            Estado = autorizacion.Element("estado")?.Value ?? "",
            NumeroAutorizacion = autorizacion.Element("numeroAutorizacion")?.Value ?? "",
            FechaAutorizacion = fechaAutorizacion,
            Ambiente = autorizacion.Element("ambiente")?.Value ?? "",
            XmlAutorizado = autorizacion.Element("comprobante")?.Value ?? "",
            Mensajes = mensajes
        };
    }

    public async Task<bool> VerificarConectividadAsync()
    {
        try
        {
            var url = _esProd ? UrlRecepcionProduccion : UrlRecepcionPruebas;
            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
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
}