using System.ComponentModel.DataAnnotations;

namespace WebApiDonCho.DTO
{
    public class FacSecuenciaDiaDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public required int Fecha { get; set; }

        [Required]
        public int secuencia { get; set; }

    }
}