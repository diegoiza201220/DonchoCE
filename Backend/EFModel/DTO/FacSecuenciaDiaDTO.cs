using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO
{
    public class FacSecuenciaDiaDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public required int Fecha { get; set; }

        [Required]
        public int secuencia { get; set; }
        public int Sucursalid { get; set; }
        public GenSucursalDTO Sucursal { get; set; } = new();
    }
}