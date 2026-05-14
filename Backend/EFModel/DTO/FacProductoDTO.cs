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

        public short? CodigoIva { get; set; }

        public bool? Activo { get; set; }

        public bool? PedidoACocina { get; set; }

        public short? OrdenAparicion { get; set; }
    }
}