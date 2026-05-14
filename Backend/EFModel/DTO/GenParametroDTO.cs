using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO
{
    public class GenParametroDTO
    {
        [Required]
        [StringLength(50)]
        public required string Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Descripcion { get; set; }

        [Required]
        [StringLength(5000)]
        public required string Valor { get; set; }
    }
}