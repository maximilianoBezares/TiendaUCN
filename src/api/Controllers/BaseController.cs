using Microsoft.AspNetCore.Mvc;

namespace TiendaUCN.src.api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController : ControllerBase
    {
        protected int GetUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault
            (
                c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier
            );
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
        }
    }
}