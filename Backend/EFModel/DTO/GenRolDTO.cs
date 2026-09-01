using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO
{
    public class GenRolDTO
    {
        public short Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }
    }
}
