namespace EFModel.Models;

public partial class GenMenuPermiso
{
    public int Id { get; set; }

    /// <summary>Id del menú padre. Null si es raíz.</summary>
    public int? Padreid { get; set; }

    public string Nombre { get; set; } = null!;

    public string? UrlRuta { get; set; }

    public string Tipo { get; set; } = null!;

    public int Orden { get; set; }

    // Autoreferencia
    public virtual GenMenuPermiso? Padre { get; set; }
    public virtual ICollection<GenMenuPermiso> Hijos { get; set; } = new List<GenMenuPermiso>();
}
