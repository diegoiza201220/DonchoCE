namespace EFModel.Models;

public partial class CelInfoTributaria
{
    public int Id { get; set; }
    public string RazonSocial { get; set; } = string.Empty;
    public string NombreComercial { get; set; } = string.Empty;
    public string Ruc { get; set; } = string.Empty;
    public string DireccionMatriz { get; set; } = string.Empty;
    public string ContribuyenteEspecial { get; set; } = string.Empty;
    public bool ObligadoContabilidad { get; set; }
    public string ContribuyenteRimpe { get; set; } = string.Empty;
}
