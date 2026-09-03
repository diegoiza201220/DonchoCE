namespace EFModel.Models;

public partial class FacDetalleOrden
{
    public int Id { get; set; }

    public int Ordenid { get; set; }

    public int Productoid { get; set; }

    public short Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }
    public decimal ImpuestoValor { get; set; }
    public decimal ImpuestoTarifa { get; set; }
    public short ImpuestoCodigo { get; set; }
    public short ImpuestoCodigoPorcentaje { get; set; }
    public decimal PrecioTotal { get; set; }
    public bool PedidoACocina { get; set; }

    public int SucursalId { get; set; }

    public virtual FacOrden Orden { get; set; } = null!;

    public virtual FacProducto Producto { get; set; } = null!;
}
