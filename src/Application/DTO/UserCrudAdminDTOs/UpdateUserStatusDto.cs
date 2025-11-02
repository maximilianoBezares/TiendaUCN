using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TiendaUCN.src.Application.DTO.UserCrudAdminDTOs
{
    public class UpdateUserStatusDto
    {
        [Required(ErrorMessage = "El estado es obligatorio.")]
        [RegularExpression("^(active|blocked)$", ErrorMessage = "El estado debe ser 'active' o 'blocked'.")]
        public required string Status { get; set; }

        public string? Reason { get; set; }
    }
}