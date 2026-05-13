using System.ComponentModel.DataAnnotations;

namespace WebApiDonCho.DTO
{
    public class FacOrdenDTO
    {
        [Required]
        public int Clienteid { get; set; }

        public int FechaInteger { get; set; }

        public int Secuencial { get; set; }

        public DateTime Fecha { get; set; }

        [Required]
        [StringLength(20)]
        public string TipoPago { get; set; }

        [Required]
        public decimal TotalOrden { get; set; }

        public decimal ValorIva { get; set; }

        public short? CodigoIva { get; set; }

        [Required]
        [StringLength(50)]
        public string UsuarioRegistro { get; set; }

        public bool EsFactura { get; set; }

        [StringLength(50)]
        public string NumeroFactura { get; set; }
        public string DocumentoPago { get; set; }

        [Required]
        public List<FacDetalleOrdenDTO> FacDetalleOrdens { get; set; } = new();
    }
}