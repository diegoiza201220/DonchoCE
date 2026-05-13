
using System.ComponentModel.DataAnnotations;

namespace WebApiDonCho.DTO
{
    public class CelLogDocumentoDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int TipoDocumento { get; set; }

        [Required]
        public int Estado { get; set; }

        [Required]
        [StringLength(50)]
        public string Mensaje { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Autorizacion { get; set; } = null!;

        [Required]
        public string XmlFirmado { get; set; } = null!;

        [Required]
        public int Ambiente { get; set; }

        [Required]
        public int TipoEmision { get; set; }
    }
}
