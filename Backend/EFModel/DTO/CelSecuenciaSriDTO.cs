using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO
{
    public class CelSecuenciaSriDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int TipoDocumento { get; set; }

        [Required]
        public string Establecimiento { get; set; } = null!;

        [Required]
        public string PuntoDeEmision { get; set; } = null!;

        [Required]
        public int SecuenciaActual { get; set; }

        [Required]
        public bool Estado { get; set; }
    }
}
