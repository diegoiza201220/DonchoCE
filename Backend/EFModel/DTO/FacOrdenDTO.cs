using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO
{
    public class FacOrdenDTO
    {
        public int Id { get; set; }

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
        public short ImpuestoPorcentaje { get; set; }
        public short ImpuestoCodigoPorcentaje { get; set; }
        public string ClaveNumeroAutorizacion { get; set; } = string.Empty;
        public string Establecimiento { get; set; } = string.Empty;
        public string PuntoEmision { get; set; } = string.Empty;

        #region Informacion tributaria para factura
        public string RazonSocial { get; set; } = string.Empty;
        public string NombreComercial { get; set; } = string.Empty;
        public string RucDonCho { get; set; } = string.Empty;
        public string Direccionmatriz { get; set; } = string.Empty;
        public string ContibuyenteRimpe { get; set; } = string.Empty;
        public string DireccionEstablecimiento { get; set; } = string.Empty;
        public string ObligadoContabilidad { get; set; } = string.Empty;
        public string CodDoc { get; set; } = string.Empty;
        public string Xml { get; set; } = string.Empty;
        #endregion

        #region NotaCredito
        public bool EsNotaCredito { get; set; } = false;
        public string NotaCreditoClaveNumeroAutorizacion { get; set; } = string.Empty;
        public string NotaCreditoNumeroNotaCredito { get; set; } = string.Empty;
        public string NotaCreditoMotivo { get; set; } = string.Empty;
        public DateTime NotaCreditoFecha { get; set; }
        #endregion NotaCredito

        [Required]
        public List<FacDetalleOrdenDTO> FacDetalleOrdens { get; set; } = new();
        public FacClienteDTO Cliente { get; set; } = new();
    }
}