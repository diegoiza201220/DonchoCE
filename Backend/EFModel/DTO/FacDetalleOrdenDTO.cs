using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO
{
    public class FacDetalleOrdenDTO
    {
        public int Id { get; set; }

        public int Ordenid { get; set; }

        [Required]
        public int ProductoId { get; set; }

        [Required]
        public short Cantidad { get; set; }

        [Required]
        public decimal PrecioUnitario { get; set; }

        public decimal ImpuestoValor { get; set; }
        public decimal ImpuestoTarifa { get; set; }
        public short ImpuestoCodigo { get; set; }
        public short ImpuestoCodigoPorcentaje { get; set; }

        public decimal PrecioTotal { get; set; }

        public bool PedidoACocina { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}