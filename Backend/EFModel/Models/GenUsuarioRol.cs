namespace EFModel.Models;

/// <summary>
/// Tabla de unión muchos-a-muchos entre GenUsuario y GenRol.
/// No tiene PK propia en la BD; se configura con clave compuesta en el contexto.
/// </summary>
public partial class GenUsuarioRol
{
    public int? Usuarioid { get; set; }

    public int? Rolid { get; set; }

    // Navegación
    public virtual GenUsuario? Usuario { get; set; }
    public virtual GenRol? Rol { get; set; }
}
