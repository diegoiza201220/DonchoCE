using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO
{
    /// <summary>
    /// DTO para asignar o quitar una sucursal a un usuario.
    /// </summary>
    public class GenUsuarioSucursalDTO
    {
        public int Id { get; set; }

        [Required]
        public int Usuarioid { get; set; }

        [Required]
        public int Sucursalid { get; set; }

        // Campos de solo lectura para mostrar nombres en listados
        public string? NombreUsuario { get; set; }
        public string? NombreSucursal { get; set; }
    }
}
