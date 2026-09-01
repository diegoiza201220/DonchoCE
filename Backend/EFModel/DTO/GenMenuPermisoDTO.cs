using System.ComponentModel.DataAnnotations;

namespace EFModel.DTO
{
    /// <summary>
    /// DTO plano para operaciones de creación y edición de un ítem de menú.
    /// No incluye hijos para evitar ciclos de serialización JSON.
    /// Usar <see cref="GenMenuPermisoTreeDTO"/> para renderizar el árbol completo.
    /// </summary>
    public class GenMenuPermisoDTO
    {
        public int Id { get; set; }

        /// <summary>Id del menú padre. Null si es nodo raíz.</summary>
        public int? Padreid { get; set; }

        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }

        [StringLength(200)]
        public string? UrlRuta { get; set; }

        [Required]
        [StringLength(50)]
        public required string Tipo { get; set; }

        [Required]
        public int Orden { get; set; }
    }

    /// <summary>
    /// DTO árbol para listar el menú completo con hijos anidados.
    /// Usar solo en lecturas; no enviar de vuelta al servidor.
    /// </summary>
    public class GenMenuPermisoTreeDTO
    {
        public int Id { get; set; }
        public int? Padreid { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? UrlRuta { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int Orden { get; set; }

        public List<GenMenuPermisoTreeDTO> Hijos { get; set; } = new();
    }
}
