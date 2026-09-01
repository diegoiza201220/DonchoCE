namespace EFModel.Models;

/// <summary>
/// Tabla de unión muchos-a-muchos entre GenUsuario y GenSucursal.
/// Ambas columnas son NOT NULL en la BD; clave compuesta configurada en el contexto.
/// </summary>
public partial class GenUsuarioSucursal
{
    public int Id { get; set; }
    public int Usuarioid { get; set; }

    public int Sucursalid { get; set; }

    // Navegación
    public required virtual GenUsuario Usuario { get; set; } 
    public required virtual GenSucursal Sucursal { get; set; } 
}
