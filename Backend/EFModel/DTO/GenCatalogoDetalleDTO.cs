using System;
using System.Collections.Generic;

namespace EFModel.Models;

public partial class GenCatalogoDetalleDTO
{
    public int Id { get; set; }
    public int Catalogoid { get; set; }
    public string Codigo { get; set; } = null!;
    public string Valor { get; set; } = null!;
    public bool Activo { get; set; }
}
