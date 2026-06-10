using System;
using System.Collections.Generic;

namespace EFModel.Models;

public partial class FacCliente
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string CedulaRuc { get; set; } = null!;

    public string? TelefonoCelular { get; set; }

    public string? Email { get; set; }

    public int FechaCumpleanios { get; set; }

    public string? Direccion { get; set; }

    public DateOnly FechaRegistro { get; set; }

    public string UsuarioRegistro { get; set; } = null!;

    public virtual ICollection<FacOrden> FacOrdens { get; set; } = new List<FacOrden>();
}
