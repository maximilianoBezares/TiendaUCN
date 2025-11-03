using System.ComponentModel.DataAnnotations;

namespace TiendaUCN.src.Application.DTO.UserProfileDTO
{
    public class UpdateUserEmailVerificationDTO
    {
        /// <summary>
        /// Correo electrónico del usuario.
        /// </summary>
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        public required string Email { get; set; }
    }
}