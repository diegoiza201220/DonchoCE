namespace EFModel.Models;

public partial class CelLogDocumento
{
    public int Id { get; set; }

    public int TipoDocumento { get; set; }

    public int Estado { get; set; }

    public string Mensaje { get; set; } = null!;

    public string Autorizacion { get; set; } = null!;

    public string XmlFirmado { get; set; } = null!;

    public int Ambiente { get; set; }

    public int TipoEmision { get; set; }

    public DateOnly FechaHora { get; set; }
}
