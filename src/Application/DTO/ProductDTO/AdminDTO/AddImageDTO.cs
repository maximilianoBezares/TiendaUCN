using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Application.DTO.ProductDTO.AdminDTO
{
    public class AddImagesDTO
    {
        [Required(ErrorMessage = "Las imágenes son obligatorias.")]
        public required List<IFormFile> Images { get; set; } = new List<IFormFile>();
    }
}
