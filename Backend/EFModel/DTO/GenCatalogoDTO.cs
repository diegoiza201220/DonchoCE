using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO
{
    public class GenCatalogoDTO
    {
        public required int Id { get; set; }
        public required string Nombre { get; set; }
        public required bool Activo { get; set; }
    }
}