using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO.Request
{
    public class RqOrdenesPorFechas
    {
        [Required]
        public int FechaIni { get; set; }

        [Required]
        public int FechaFin { get; set; }

    }

    public class RqConsultas
    {
        public string ValorString1 { get; set; } = string.Empty;

        public string ValorString2 { get; set; } = string.Empty;

    }
}