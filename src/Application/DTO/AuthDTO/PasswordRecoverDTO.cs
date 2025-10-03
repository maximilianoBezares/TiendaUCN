using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TiendaUCN.src.Application.DTO.AuthDTO
{
    public class PasswordRecoverDTO
    {
        /// <summary>
        /// Email del usuario.
        /// </summary>
        [Required(ErrorMessage = "El email es requerido.")]
        [EmailAddress(ErrorMessage = "El Correo electrónico no es válido.")]
        public required string Email { get; set; }        
    }
}