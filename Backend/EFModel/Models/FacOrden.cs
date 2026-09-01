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
    public decimal TotalSinImpuestos { get; set; }

    public decimal ImpuestoValor { get; set; }
    public decimal ImpuestoBaseImponible { get; set; }
    public short ImpuestoCodigo { get; set; }
    public short ImpuestoCodigoPorcentaje { get; set; }
    public short ImpuestoPorcentaje { get; set; }

    public string UsuarioRegistro { get; set; } = null!;
    public bool EsFactura { get; set; }

    public string? NumeroFactura { get; set; }
    public string ClaveNumeroAutorizacion { get; set; } = string.Empty;
    public string Establecimiento { get; set; } = string.Empty;
    public string PuntoEmision { get; set; } = string.Empty;
    public string DocumentoPago { get; set; } = null!;

    public bool EsNotaCredito { get; set; } = false;
    public string NotaCreditoClaveNumeroAutorizacion { get; set; } = string.Empty;
    public string NotaCreditoNumeroNotaCredito { get; set; } = string.Empty;
    public string NotaCreditoMotivo { get; set; } = string.Empty;
    public DateTime NotaCreditoFecha { get; set; }
    public virtual FacCliente Cliente { get; set; } = null!;

    public virtual ICollection<FacDetalleOrden> FacDetalleOrdens { get; set; } = [];

    public int Sucursalid { get; set; }

    public virtual GenSucursal Sucursal { get; set; } = null!;
}
