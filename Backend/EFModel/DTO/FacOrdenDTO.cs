using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO
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

        [Required]
        [StringLength(50)]
        public string UsuarioRegistro { get; set; }

        public bool EsFactura { get; set; }

        [StringLength(50)]
        public string NumeroFactura { get; set; }
        public string DocumentoPago { get; set; }

        public decimal TotalSinImpuestos { get; set; }
        public decimal ImpuestoValor { get; set; }
        public decimal ImpuestoBaseImponible { get; set; }
        public short ImpuestoCodigo { get; set; }
        public short ImpuestoCodigoPorcentaje { get; set; }
        public string ClaveNumeroAutorizacion { get; set; } = string.Empty;
        public string Establecimiento { get; set; } = string.Empty;
        public string PuntoEmision { get; set; } = string.Empty;
        [Required]
        public List<FacDetalleOrdenDTO> FacDetalleOrdens { get; set; } = new();
    }
}