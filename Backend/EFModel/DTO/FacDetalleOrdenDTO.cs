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

        public decimal ValorIva { get; set; }

        public short CodigoIva { get; set; }

        public decimal PrecioTotal { get; set; }

        public bool PedidoACocina { get; set; }
    }
}