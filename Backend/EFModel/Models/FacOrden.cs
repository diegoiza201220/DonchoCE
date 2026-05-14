using System;
using System.Collections.Generic;

namespace EFModel.Models;

public partial class FacOrden
{
    public int Id { get; set; }

    public int Clienteid { get; set; }

    public int FechaInteger { get; set; }

    public int Secuencial { get; set; }

    public DateTime Fecha { get; set; }

    public string TipoPago { get; set; } = null!;

    public decimal TotalOrden { get; set; }

    public decimal ValorIva { get; set; }

    public short? CodigoIva { get; set; }

    public string UsuarioRegistro { get; set; } = null!;

    public bool EsFactura { get; set; }

    public string? NumeroFactura { get; set; }
    public string DocumentoPago { get; set; } = null!;

    public virtual FacCliente Cliente { get; set; } = null!;

    public virtual ICollection<FacDetalleOrden> FacDetalleOrdens { get; set; } = new List<FacDetalleOrden>();
}
