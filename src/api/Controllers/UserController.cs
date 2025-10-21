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
            return Ok(new GenericResponse<UserProfileDataDTO>("Perfil de usuario obtenido exitosamente", result));

        }
    }
}