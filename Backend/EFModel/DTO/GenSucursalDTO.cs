using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO
{
    public class GenSucursalDTO
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(200)]
        public string Direccion { get; set; } = string.Empty;

        public bool EsMatriz { get; set; } = false;

        public bool Activo { get; set; } = true;
    }
}
