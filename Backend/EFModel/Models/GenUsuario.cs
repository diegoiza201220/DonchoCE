namespace EFModel.Models;

public partial class GenUsuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Password { get; set; } = null!;
}
