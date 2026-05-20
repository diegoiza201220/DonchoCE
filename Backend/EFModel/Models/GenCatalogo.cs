using System;
using System.Collections.Generic;

namespace EFModel.Models;

public partial class GenCatalogo
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public bool Activo { get; set; }
    public virtual ICollection<GenCatalogoDetalle> CatalogoDetalles { get; set; } = new List<GenCatalogoDetalle>();
}
