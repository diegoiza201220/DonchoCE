namespace EFModel.Models;

public partial class CelCertificado
{
    public int Id { get; set; }

    public string Firma { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public string NombreCertificado { get; set; } = null!;

    public bool Activo { get; set; }
}
