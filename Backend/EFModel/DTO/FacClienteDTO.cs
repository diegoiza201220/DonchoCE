using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO
{
    public class FacClienteDTO
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [StringLength(100)]
        public string Apellido { get; set; } = null!;

        [StringLength(20)]
        public string CedulaRuc { get; set; } = null!;

        [StringLength(20)]
        public string? TelefonoCelular { get; set; }

        [StringLength(50)]
        public string? Email { get; set; }

        public int FechaCumpleanios { get; set; }

        [StringLength(200)]
        public string? Direccion { get; set; }

        public DateTime FechaRegistro { get; set; }

        [StringLength(20)]
        public string UsuarioRegistro { get; set; } = null!;

    }
}