// DTO/Sri/RespuestaRecepcionSri.cs
namespace ComprobantesElectronicos.DTO.Sri;

public class ResultadoEmisionDTO
{
    public bool Exitoso { get; set; }
    public string NumeroAutorizacion { get; set; } = "";
    public DateTime FechaAutorizacion { get; set; }
    public string XmlAutorizado { get; set; } = "";
    public List<string> Mensajes { get; set; } = new();
}

public class RespuestaRecepcionSri
{
    public string Estado { get; set; } = "";            // RECIBIDA | DEVUELTA
    public List<MensajeSri> Mensajes { get; set; } = new();
    public bool FueRecibida => Estado == "RECIBIDA";
}

public class RespuestaAutorizacionSri
{
    public string Estado { get; set; } = ""; // AUTORIZADO | NO AUTORIZADO
    public string NumeroAutorizacion { get; set; } = "";
    public DateTime FechaAutorizacion { get; set; }
    public string Ambiente { get; set; } = "";
    public string XmlAutorizado { get; set; } = "";
    public List<MensajeSri> Mensajes { get; set; } = new();
    public bool FueAutorizado => Estado == "AUTORIZADO";
}

public class MensajeSri
{
    public string Identificador { get; set; } = "";
    public string Mensaje { get; set; } = "";
    public string Tipo { get; set; } = ""; // ERROR | ADVERTENCIA
    public string Informacion { get; set; } = "";
}