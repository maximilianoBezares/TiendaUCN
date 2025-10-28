using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaUCN.src.api.Controllers;
using TiendaUCN.src.Application.DTO;
using TiendaUCN.src.Application.DTO.OrderDTO;
using TiendaUCN.src.Application.DTO.OrderDTO.AdminDTO;
using TiendaUCN.src.Application.DTO.ProductDTO;
using TiendaUCN.src.Application.Services.Interfaces;

namespace TiendaUCN.src.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de órdenes.
    /// </summary>
    public class OrderController : BaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Crea una nueva orden.
        /// </summary>
        /// <returns>Detalles de la orden creada.</returns>
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateOrder()
        {
            var userId =
                (
                    User.Identity?.IsAuthenticated == true
                        ? User
                            .Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                            ?.Value
                        : null
                ) ?? throw new UnauthorizedAccessException("Usuario no autenticado.");
            int.TryParse(userId, out int parsedUserId);
            var result = await _orderService.CreateAsync(parsedUserId);
            return Created(
                $"api/order/detail/{result}",
                new GenericResponse<string>("Orden creada exitosamente", result)
            );
        }

        /// <summary>
        /// Obtiene los detalles de una orden.
        /// </summary>
        /// <param name="orderCode">Código de la orden</param>
        /// <returns>Detalles de la orden encontrada.</returns>
        [HttpGet("detail/{orderCode}")]
        [Authorize]
        public async Task<IActionResult> GetOrderDetail(string orderCode)
        {
            var result = await _orderService.GetDetailAsync(orderCode);
            return Ok(
                new GenericResponse<OrderDetailDTO>(
                    "Detalle de orden obtenido exitosamente",
                    result
                )
            );
        }

        /// <summary>
        /// Obtiene las órdenes de un usuario.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda</param>
        /// <returns>Órdenes del usuario.</returns>
        [HttpGet("user-orders")]
        [Authorize]
        public async Task<IActionResult> GetUserOrders([FromQuery] SearchParamsDTO searchParams)
        {
            var userId =
                (
                    User.Identity?.IsAuthenticated == true
                        ? User
                            .Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)
                            ?.Value
                        : null
                ) ?? throw new UnauthorizedAccessException("Usuario no autenticado.");
            int.TryParse(userId, out int parsedUserId);
            var result = _orderService.GetByUserIdAsync(searchParams, parsedUserId);
            return Ok(
                new GenericResponse<ListedOrderDetailDTO>(
                    "Órdenes del usuario obtenidas exitosamente",
                    await result
                )
            );
        }

        /// <summary>
        ///Obtiene una lista paginada y filtrada de todas las órdenes.
        /// </summary>
        [HttpGet("users-orders/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetFilteredForAdmin(
            [FromQuery] AdminOrderSearchParamsDTO searchParams
        )
        {
            var result = await _orderService.GetFilteredForAdminAsync(searchParams);
            return Ok(
                new GenericResponse<ListedAdminOrdersDTO>("Órdenes obtenidas exitosamente", result)
            );
        }

        /// <summary>
        ///Obtiene el detalle completo de una orden específica.
        /// </summary>
        /// <param name="id">El ID de la orden.</param>
        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdForAdmin(int id)
        {
            var result = await _orderService.GetByIdForAdminAsync(id);
            return Ok(
                new GenericResponse<AdminOrderDetailDTO>(
                    "Detalle de orden obtenido exitosamente",
                    result
                )
            );
        }

        /// <summary>
        ///Actualiza el estado de una orden.
        /// </summary>
        /// <param name="id">El ID de la orden a actualizar.</param>
        /// <param name="dto">El DTO con el nuevo estado.</param>
        [HttpPatch("admin/{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusDTO dto)
        {
            var adminId =
                User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException(
                    "No se pudo identificar al administrador."
                );

            await _orderService.UpdateStatusAsync(id, dto, adminId);
            return Ok(
                new GenericResponse<string>(
                    "Estado actualizado",
                    $"El estado de la orden {id} ha sido actualizado a {dto.Status}."
                )
            );
        }
    }
}
