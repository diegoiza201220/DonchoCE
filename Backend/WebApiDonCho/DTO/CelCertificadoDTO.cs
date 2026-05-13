using System.ComponentModel.DataAnnotations;

namespace WebApiDonCho.DTO
{
    public class CelCertificadoDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Firma { get; set; } = null!;

        public string Clave { get; set; } = null!;

        public string NombreCertificado { get; set; } = null!;

        public bool Activo { get; set; }
    }
}
