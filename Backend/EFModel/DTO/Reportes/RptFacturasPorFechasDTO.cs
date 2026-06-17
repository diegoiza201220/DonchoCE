namespace EFModel.DTO.Reportes
{
    public class RptFacturasPorFechasDTO
    {
        public DateTime Fecha { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public int Secuencial { get; set; }
        public string TipoPago { get; set; } = string.Empty;
        public decimal TotalSinImpuestos { get; set; }
        public decimal ImpuestoValor { get; set; }
        public decimal ImpuestoPorcentaje { get; set; }
        public decimal TotalOrden { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public string NumeroAutorizacion { get; set; } = string.Empty;
        public string Establecimiento { get; set; } = string.Empty;
        public string PuntoEmision { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }
}