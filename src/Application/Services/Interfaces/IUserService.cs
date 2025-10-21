using TiendaUCN.src.Application.DTO;
using TiendaUCN.src.Application.DTO.AuthDTO;
using TiendaUCN.src.Application.DTO.UserProfileDTO;

namespace TiendaUCN.src.Application.Services.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de usuarios.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Inicia sesión con el usuario proporcionado.
        /// </summary>
        /// <param name="loginDTO">DTO que contiene las credenciales del usuario.</param>
        /// <param name="httpContext">El contexto HTTP actual.</param>
        /// <returns>Un string que representa el token JWT generadon y la id del usuario.</returns>
        Task<(string token, int userId)> LoginAsync(LoginDTO loginDTO, HttpContext httpContext);

        /// <summary>
        /// Elimina usuarios no confirmados.
        /// </summary>
        /// <returns>Número de usuarios eliminados</returns>
        Task<int> DeleteUnconfirmedAsync();

        /// <summary>
        /// Registra un nuevo usuario.
        /// </summary>
        /// <param name="registerDTO">DTO que contiene la información del nuevo usuario.</param>
        /// <param name="httpContext">El contexto HTTP actual.</param>
        /// <returns>Un string que representa el mensaje de éxito del registro.</returns>
        Task<string> RegisterAsync(RegisterDTO registerDTO, HttpContext httpContext);

        /// <summary>
        /// Verifica el correo electrónico del usuario.
        /// </summary>
        /// <param name="verifyEmailDTO">DTO que contiene el correo electrónico y el código de verificación.</param>
        /// <returns>Un string que representa el mensaje de éxito de la verificación.</returns>
        Task<string> VerifyEmailAsync(VerifyEmailDTO verifyEmailDTO);

        /// <summary>
        /// Reenvía el código de verificación al correo electrónico del usuario.
        /// </summary>
        /// <param name="resendEmailVerificationCodeDTO">DTO que contiene el correo electrónico del usuario.</param>
        /// <returns>Un string que representa el mensaje de éxito del reenvío.</returns>
        Task<string> ResendEmailVerificationCodeAsync(ResendEmailVerificationDTO resendEmailVerificationCodeDTO);

        /// <summary>
        /// Inicia el proceso de recuperación de contraseña.
        /// </summary>
        /// <param name="passwordRecoverDTO">DTO que contiene el correo electrónico del usuario.</param>
        /// <param name="httpContext">El contexto HTTP actual.</param>
        /// <returns>Un string que representa el mensaje de éxito del inicio de la recuperación de contraseña.</returns>
        Task<string> PasswordRecoverAsync(PasswordRecoverDTO passwordRecoverDTO, HttpContext httpContext);

        /// <summary>
        /// Restablece la contraseña del usuario.
        /// </summary>
        /// <param name="resetPasswordDTO">DTO que contiene el correo electrónico, el código
        /// y la nueva contraseña del usuario.</param>
        /// <returns>Un string que representa el mensaje de éxito del restablecimiento de la
        Task<string> ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);

        /// <summary>
        /// Obtiene el perfil de un usuario por su ID.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <returns>Un UserProfileDataDTO que representa el perfil del usuario.</returns>
        Task<UserProfileDataDTO> GetUserProfileAsync(int userId);
    }
}