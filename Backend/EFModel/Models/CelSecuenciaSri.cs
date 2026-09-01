namespace EFModel.Models;

public partial class CelSecuenciaSri
{
    public int Id { get; set; }

    public string TipoDocumento { get; set; } = string.Empty;

    public string Establecimiento { get; set; } = null!;

    public string PuntoDeEmision { get; set; } = null!;

    public int SecuenciaActual { get; set; }

    public bool Estado { get; set; }
    public int Sucursalid { get; set; }

    public virtual GenSucursal Sucursal { get; set; } = null!;
}
