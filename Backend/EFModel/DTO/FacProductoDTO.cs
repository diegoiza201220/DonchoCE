using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO
{
    public class FacProductoDTO
    {
        [Required]
        public int Id { get; set; }

        [StringLength(100)]
        public required string Nombre { get; set; }

        [StringLength(50)]
        public required string Grupo { get; set; }

        public decimal Valor { get; set; }

        public int CodigoIva { get; set; }
        public string IvaTarifa { get; set; } = string.Empty;
        public decimal IvaValor { get; set; }
        public decimal ValorTotal { get; set; }

        public bool Activo { get; set; }

        public bool? PedidoACocina { get; set; }

        public short? OrdenAparicion { get; set; }

        public decimal ValorDoncho { get; set; }
    }
}