using System.ComponentModel.DataAnnotations;

namespace WebApiDonCho.DTO
{
    public class GenUsuarioDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public required string Nombre { get; set; }

        [Required]
        [StringLength(50)]
        public required string Password { get; set; }
    }
}