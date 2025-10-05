using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TiendaUCN.src.Application.DTO.AuthDTO
{
    public class ResetPasswordDTO
    {
        /// <summary>
        /// Email del usuario.
        /// </summary>
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        public required string Email { get; set; }

        /// <summary>
        /// Código de verificación enviado al correo electrónico.
        /// </summary>
        [Required(ErrorMessage = "El código de verificación es obligatorio.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "El código de verificación debe tener 6 dígitos.")]
        public required string VerificationCode { get; set; }

        /// <summary>
        /// Nueva contraseña del usuario.
        /// </summary>
        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La nueva contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).+$",
        ErrorMessage = "La contraseña debe incluir mayúscula, minúscula, número y caracter especial.")]
        public required string NewPassword { get; set; }

        /// <summary>
        /// Confirma la nueva contraseña del usuario.
        /// </summary>
        [Required(ErrorMessage = "La confirmación de la nueva contraseña es obligatoria.")]
        [Compare("NewPassword", ErrorMessage = "La confirmación de la nueva contraseña no coincide.")]
        public required string ConfirmNewPassword { get; set; }
    }
}