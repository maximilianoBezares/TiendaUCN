using System.ComponentModel.DataAnnotations;

namespace TiendaUCN.src.Application.DTO.BrandDTO
{
    public class BrandUpdateDTO
    {
        /// <summary>
        /// Nombre de la marca
        /// </summary>
        [Required(ErrorMessage = "El nombre de la marca es obligatorio.")]
        [StringLength(80, ErrorMessage = "El nombre de la marca no puede exceder los 80 caracteres.")]
        [MinLength(2, ErrorMessage = "El nombre de la marca debe tener al menos 2 caracteres.")]
        public required string name { get; set; }

        /// <summary>
        /// Descripcion de la marca
        /// </summary>
        public string? description { get; set; }
    }
}