using System;
using System.ComponentModel.DataAnnotations;

namespace TiendaUCN.src.Application.DTO.UserProfileDTO
{
    public class UserProfileDataDTO
    {

        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Apellido del usuario.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Género del usuario.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Fecha de nacimiento del usuario.
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Rut del usuario.
        /// </summary>
        public string Rut { get; set; }

        /// <summary>
        /// Email del usuario.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Número de teléfono del usuario.
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}