using System.ComponentModel.DataAnnotations;

namespace TiendaUCN.src.Application.DTO.UserProfileDTO
{
    public class ChangePasswordDTO
    {
        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        [Required(ErrorMessage = "La contraseña es requerida.")]

        public required string CurrentPassword { get; set; }

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