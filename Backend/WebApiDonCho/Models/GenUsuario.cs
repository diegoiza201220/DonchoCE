using System;
using System.Collections.Generic;

namespace WebApiDonCho.Models;

public partial class GenUsuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Password { get; set; } = null!;
}
