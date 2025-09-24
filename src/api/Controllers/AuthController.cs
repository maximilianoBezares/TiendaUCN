using Microsoft.AspNetCore.Mvc;
using Serilog;
using TiendaUCN.src.Application.DTO;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Application.DTO.AuthDTO;
using TiendaUCN.src.Application.Services.Interfaces;

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
    }
}