using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;

namespace TiendaUCN.src.Application.DTO.ProductDTO.AdminDTO
{
    public class UpdateDiscountDTO
    {
        [Required(ErrorMessage = "El descuento es obligatorio.")]
        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100.")]
        public int Discount { get; set; }
    }
}
