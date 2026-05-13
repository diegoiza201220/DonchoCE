using System.ComponentModel.DataAnnotations;

namespace WebApiDonCho.DTO.Request
{
    public class RqOrdenesPorFechas
    {
        [Required]
        public int FechaIni { get; set; }

        [Required]
        public int FechaFin { get; set; }

    }
}