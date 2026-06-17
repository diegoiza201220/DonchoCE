namespace EFModel.DTO.Reportes
{
    public class RptOrdenesPorFechasDTO
    {
        public int ClienteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;

        public int FechaInteger { get; set; }

        public int Secuencial { get; set; }

        public DateTime Fecha { get; set; }

        public string TipoPago { get; set; } = string.Empty;

        public decimal TotalOrden { get; set; }

        public decimal ValorIva { get; set; }

        public short CodigoIva { get; set; }

        public string UsuarioRegistro { get; set; } = string.Empty;

        public bool EsFactura { get; set; }

        public string NumeroFactura { get; set; } = string.Empty;
        public string DocumentoPago { get; set; } = string.Empty;

    }
}