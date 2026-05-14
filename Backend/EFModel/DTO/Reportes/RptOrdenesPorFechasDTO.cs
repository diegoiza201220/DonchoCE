using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO.Reportes
{
    public class RptOrdenesPorFechasDTO
    {
        public int ClienteId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }

        public int FechaInteger { get; set; }

        public int Secuencial { get; set; }

        public DateTime Fecha { get; set; }

        public string TipoPago { get; set; }

        public decimal TotalOrden { get; set; }

        public decimal ValorIva { get; set; }

        public short CodigoIva { get; set; }

        public string UsuarioRegistro { get; set; }

        public bool EsFactura { get; set; }

        public string NumeroFactura { get; set; }
        public string DocumentoPago { get; set; } 

    }
}