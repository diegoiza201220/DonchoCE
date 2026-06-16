using System;
using System.Collections.Generic;

namespace EFModel.Models;

public partial class FacProducto
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Grupo { get; set; }

    public decimal Valor { get; set; }

    public int CodigoIva { get; set; }

    public bool Activo { get; set; }

    public short? OrdenAparicion { get; set; }

    public bool? PedidoACocina { get; set; }
    public decimal ValorDoncho { get; set; }

    //public virtual ICollection<FacDetalleOrden> FacDetalleOrdens { get; set; } = new List<FacDetalleOrden>();
}
