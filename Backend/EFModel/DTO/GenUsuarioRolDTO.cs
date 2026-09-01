using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO
{
    /// <summary>
    /// DTO para asignar o quitar un rol a un usuario.
    /// </summary>
    public class GenUsuarioRolDTO
    {
        [Required]
        public int Usuarioid { get; set; }

        [Required]
        public int Rolid { get; set; }

        // Campos de solo lectura para mostrar nombres en listados
        public string? NombreUsuario { get; set; }
        public string? NombreRol { get; set; }
    }
}
