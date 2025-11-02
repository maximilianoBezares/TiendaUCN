using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TiendaUCN.src.Application.DTO.UserCrudAdminDTOs
{
    public class UpdateUserRoleDto
    {
        [Required(ErrorMessage = "El rol es obligatorio.")]
        [RegularExpression("^(Admin|Customer)$", ErrorMessage = "El rol debe ser 'Admin' o 'Customer'.")]
        public required string Role { get; set; }
    }
}