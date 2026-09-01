namespace EFModel.Models;

public partial class GenSucursal
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Direccion { get; set; } = null!;

    public bool EsMatriz { get; set; }

    public bool Activo { get; set; }

    //// Navegación inversa
    public virtual ICollection<FacOrden> FacOrdens { get; set; } = new List<FacOrden>();
    public virtual ICollection<FacSecuenciaDia> FacSecuenciasDia { get; set; } = new List<FacSecuenciaDia>();
    public virtual ICollection<CelSecuenciaSri> CelSecuenciasSri { get; set; } = new List<CelSecuenciaSri>();
    public virtual ICollection<GenUsuarioSucursal> UsuarioSucursales { get; set; } = new List<GenUsuarioSucursal>();
}
