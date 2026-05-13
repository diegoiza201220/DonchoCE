using System;
using System.Collections.Generic;

namespace WebApiDonCho.Models;

public partial class GenParametro
{
    public string Id { get; set; } = null!;

    public string Descripcion { get; set; } = null!;

    public string Valor { get; set; } = null!;
}
