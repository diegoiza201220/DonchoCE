namespace EFModel.Models;

public partial class FacSecuenciaDia
{
    public int Id { get; set; }

    public int Fecha { get; set; }

    public int Secuencia { get; set; }

    public int Sucursalid { get; set; }

    public virtual GenSucursal Sucursal { get; set; } = null!;
}
