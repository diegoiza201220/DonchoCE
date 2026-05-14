using System;
using System.Collections.Generic;

namespace EFModel.Models;

public partial class FacDetalleOrden
{
    public int Id { get; set; }

    public int Ordenid { get; set; }

    public int Productoid { get; set; }

    public short Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal ValorIva { get; set; }

    public int CodigoIva { get; set; }

    public decimal PrecioTotal { get; set; }

    public bool PedidoACocina { get; set; }

    public virtual FacOrden Orden { get; set; } = null!;

    public virtual FacProducto Producto { get; set; } = null!;
}
