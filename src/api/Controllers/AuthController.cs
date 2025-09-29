using Microsoft.AspNetCore.Mvc;
using Serilog;
using TiendaUCN.src.Application.DTO;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Application.DTO.AuthDTO;


namespace TiendaUCN.src.api.Controllers
{
    /// <summary>
    /// Controlador de autenticación.
    /// </summary>
    public class AuthController(IUserService userService) : BaseController
    {
        /// <summary>
        /// Servicio de usuarios.
        /// </summary>
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Inicia sesión con el usuario proporcionado.
        /// </summary>
        /// <param name="loginDTO">DTO que contiene las credenciales del usuario.</param>
        /// <returns>Un IActionResult que representa el resultado de la operación.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var (token, userId) = await _userService.LoginAsync(loginDTO, HttpContext);
            return Ok(new GenericResponse<string>("Inicio de sesión exitoso", token));
        }

        /// <summary>
        /// Registra un nuevo usuario.
        /// </summary>
        /// <param name="registerDTO">DTO que contiene la información del nuevo usuario.</param
        /// <returns>Un IActionResult que representa el resultado de la operación.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            var message = await _userService.RegisterAsync(registerDTO, HttpContext);
            return Ok(new GenericResponse<string>("Registro exitoso", message));
        }
        
        /// <summary>
        /// Verifica el correo electrónico del usuario.
        /// </summary>
        /// <param name="verifyEmailDTO">DTO que contiene el correo electrónico y el código de verificación.</param>
        /// <returns>Un IActionResult que representa el resultado de la operación.</returns>
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDTO verifyEmailDTO)
        {
            var message = await _userService.VerifyEmailAsync(verifyEmailDTO);
            return Ok(new GenericResponse<string>("Verificación de correo electrónico exitosa", message));
        }
    }
}