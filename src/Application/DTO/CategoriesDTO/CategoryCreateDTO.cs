using System.ComponentModel.DataAnnotations;

namespace TiendaUCN.src.Application.DTO.CategoriesDTO
{
    public class CategoryCreateDTO
    {
        /// <summary>
        /// Nombre de la categoria
        /// </summary>
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(80, ErrorMessage = "El nombre de la categoría no puede exceder los 80 caracteres.")]
        [MinLength(2, ErrorMessage = "El nombre de la categoría debe tener al menos 2 caracteres.")]
        public required string name { get; set; }

        /// <summary>
        /// Descripcion de la categoria.
        /// </summary>
        public string? description { get; set; }
    }
}