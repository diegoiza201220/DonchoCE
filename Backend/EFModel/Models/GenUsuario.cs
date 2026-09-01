namespace EFModel.Models;

public partial class GenUsuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Password { get; set; } = null!;
    //public virtual ICollection<GenUsuarioRol> UsuarioRoles { get; set; } = new List<GenUsuarioRol>();

    public virtual ICollection<GenUsuarioSucursal> UsuarioSucursales { get; set; } = new List<GenUsuarioSucursal>();
}
