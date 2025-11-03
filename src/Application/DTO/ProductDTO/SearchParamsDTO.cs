using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TiendaUCN.src.Application.DTO.ProductDTO
{
    public class SearchParamsDTO
    {
        [Required(ErrorMessage = "El número de página es obligatorio.")]
        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "El número de página debe ser un número entero positivo."
        )]
        public int PageNumber { get; set; }

        [Range(
            1,
            int.MaxValue,
            ErrorMessage = "El tamaño de página debe ser un número entero positivo."
        )]
        public int? PageSize { get; set; }

        [MinLength(2, ErrorMessage = "El término de búsqueda debe tener al menos 2 caracteres.")]
        [MaxLength(40, ErrorMessage = "El término de búsqueda no puede exceder los 40 caracteres.")]
        public string? SearchTerm { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El precio mínimo debe ser un valor positivo.")]
        public int? MinPrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El precio máximo debe ser un valor positivo.")]
        public int? MaxPrice { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El ID de categoría debe ser un número positivo.")]
        public int? CategoryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El ID de marca debe ser un número positivo.")]
        public int? BrandId { get; set; }

        [Range(
            typeof(bool),
            "false",
            "true",
            ErrorMessage = "Debes aceptar los términos y condiciones para continuar."
        )]
        public bool? IsDelete { get; set; }

        [Range(
            typeof(bool),
            "false",
            "true",
            ErrorMessage = "Debes aceptar los términos y condiciones para continuar."
        )]
        public bool? IsActive { get; set; }
    }
}
