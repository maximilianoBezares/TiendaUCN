using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using TiendaUCN.src.Application.DTO;
using TiendaUCN.src.Application.DTO.UserCrudAdminDTOs;
using TiendaUCN.src.Application.DTO.UserProfileDTO;
using TiendaUCN.src.Application.Services.Interfaces;

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
            return Ok(
                new GenericResponse<UpdateProfileDTO>(
                    "Perfil de usuario obtenido exitosamente",
                    result
                )
            );
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateUserProfileAsync(
            [FromBody] UpdateProfileDTO updateProfileDTO
        )
        {
            var userId = GetUserId();
            var result = await _userService.UpdateUserProfileAsync(userId, updateProfileDTO);
            return Ok(
                new GenericResponse<string>("Perfil de usuario actualizado exitosamente", result)
            );
        }

        [HttpPost("update-email")]
        [Authorize]
        public async Task<IActionResult> UpdateUserEmailAsync(
            [FromBody] UpdateUserEmailVerificationDTO updateUserEmailDTO
        )
        {
            var userId = GetUserId();
            var message = await _userService.SendUpdateUserEmailVerificationCodeAsync(
                userId,
                updateUserEmailDTO,
                HttpContext
            );
            return Ok(
                new GenericResponse<string>("Correo electrónico actualizado exitosamente", message)
            );
        }

        [HttpPatch("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync(
            [FromBody] ChangePasswordDTO changePasswordDTO
        )
        {
            var userId = GetUserId();
            var result = await _userService.ChangePasswordAsync(userId, changePasswordDTO);
            return Ok(new GenericResponse<string>("Contraseña cambiada exitosamente", result));
        }

        /// <summary>
        /// [Admin] Obtiene una lista paginada de usuarios (R129)
        /// </summary>
        [HttpGet("admin/users")] // Ruta absoluta
        [Authorize(Roles = "Admin")] // R128
        public async Task<IActionResult> GetUsers([FromQuery] AdminUserQueryParameters queryParams)
        {
            var result = await _userService.GetUsersAsync(queryParams);
            return Ok(
                new GenericResponse<PagedResult<UserListDto>>("Lista de usuarios obtenida", result)
            );
        }

        /// <summary>
        /// [Admin] Obtiene el detalle de un usuario específico (R132)
        /// </summary>
        [HttpGet("admin/users/{id:int}")] // Ruta absoluta
        [Authorize(Roles = "Admin")] // R128
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _userService.GetUserDetailAsync(id);
            return Ok(new GenericResponse<UserDetailDto>("Detalle de usuario obtenido", result));
        }

        /// <summary>
        /// Actualiza el estado de un usuario (bloqueado/activo)
        /// </summary>
        [HttpPatch("admin/users/{id:int}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserStatus(
            int id,
            [FromBody] UpdateUserStatusDto statusDto
        )
        {
            var adminId = GetUserId();
            var result = await _userService.UpdateUserStatusAsync(adminId, id, statusDto);
            return Ok(new GenericResponse<string>("Estado del usuario actualizado", result));
        }

        /// <summary>
        /// Actualiza el rol de un usuario
        /// </summary>
        [HttpPatch("admin/users/{id:int}/role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(
            int id,
            [FromBody] UpdateUserRoleDto roleDto
        )
        {
            try
            {
                var adminId = GetUserId();

                await _userService.UpdateUserRoleAsync(adminId, id, roleDto);

                return Ok(
                    new GenericResponse<string>(
                        "Rol del usuario actualizado exitosamente.",
                        $"Rol del usuario actualizado."
                    )
                );
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new GenericResponse<string>(ex.Message, null));
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new GenericResponse<string>(ex.Message, null));
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error actualizando rol para usuario {id}");
                return StatusCode(
                    500,
                    new GenericResponse<string>("Ocurrió un error inesperado.", null)
                );
            }
        }
    }
}
