using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TiendaUCN.src.Application.DTO.UserProfileDTO;
using TiendaUCN.src.Application.DTO;
using TiendaUCN.src.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace TiendaUCN.src.api.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetUserProfileAsync()
        {
            var userId = GetUserId();
            var result = await _userService.GetUserProfileAsync(userId);
            return Ok(new GenericResponse<UpdateProfileDTO>("Perfil de usuario obtenido exitosamente", result));

        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfileAsync([FromBody] UpdateProfileDTO updateProfileDTO)
        {
            var userId = GetUserId();
            var result = await _userService.UpdateUserProfileAsync(userId, updateProfileDTO);
            return Ok(new GenericResponse<string>("Perfil de usuario actualizado exitosamente", result));
        }

        [HttpPost("update-email")]
        [Authorize]
        public async Task<IActionResult> UpdateUserEmailAsync([FromBody] UpdateUserEmailVerificationDTO updateUserEmailDTO)
        {
            var userId = GetUserId();
            var message = await _userService.SendUpdateUserEmailVerificationCodeAsync(userId, updateUserEmailDTO, HttpContext);
            return Ok(new GenericResponse<string>("Correo electrónico actualizado exitosamente", message));
        }

        [HttpPatch("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            var userId = GetUserId();
            var result = await _userService.ChangePasswordAsync(userId, changePasswordDTO);
            return Ok(new GenericResponse<string>("Contraseña cambiada exitosamente", result));
        }
    }
}