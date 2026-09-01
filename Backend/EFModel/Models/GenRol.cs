namespace EFModel.Models;

public partial class GenRol
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    // Navegación inversa
    //public virtual ICollection<GenUsuarioRol> UsuarioRoles { get; set; } = new List<GenUsuarioRol>();
}
