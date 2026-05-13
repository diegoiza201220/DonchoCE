using System.ComponentModel.DataAnnotations;

namespace WebApiDonCho.DTO
{
    public class FacClienteDTO
    {
        [Required]
        public int Id { get; set; }

        [StringLength(100)]
        [Required]
        public string Nombre { get; set; } = null!;

        [StringLength(100)]
        [Required]
        public string Apellido { get; set; } = null!;

        [StringLength(20)]
        [Required]
        public string CedulaRuc { get; set; } = null!;

        [StringLength(20)]
        public string? TelefonoCelular { get; set; }

        [StringLength(50)]
        public string? Email { get; set; }

        public DateOnly? FechaCumpleanios { get; set; }
        
        [StringLength(200)]
        public string? Direccion { get; set; }

        public DateTime FechaRegistro { get; set; }

        [StringLength(20)]
        public string UsuarioRegistro { get; set; } = null!;

    }
}