using Mapster;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Serilog;
using TiendaUCN.src.Application.DTO.CartDTO;
using TiendaUCN.src.Application.DTO.OrderDTO;
using TiendaUCN.src.Application.DTO.OrderDTO.AdminDTO;
using TiendaUCN.src.Application.DTO.ProductDTO;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUCN.src.Application.Services.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        private readonly int _defaultPageSize;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IConfiguration configuration
        )
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _configuration = configuration;
            _defaultPageSize = int.Parse(
                _configuration["Products:DefaultPageSize"]
                    ?? throw new InvalidOperationException(
                        "La configuración 'DefaultPageSize' no está definida."
                    )
            );
        }

        /// <summary>
        /// Crea una nueva orden y vacía el carrito de compras.
        /// </summary>
        /// <param name="userId">Id del usuario autenticado</param>
        /// <returns>Crea una nueva orden y vacía el carrito de compras.</returns>
        public async Task<string> CreateAsync(int userId)
        {
            Cart? cart =
                await _cartRepository.GetByUserIdAsync(userId)
                ?? throw new KeyNotFoundException("Carrito no encontrado");
            if (cart.CartItems.Count == 0)
            {
                Log.Information("El carrito del usuario con id {userId} está vacío.", userId);
                throw new InvalidOperationException("El carrito del usuario está vacío.");
            }
            string code = await GenerateOrderCodeAsync();
            Order order = cart.Adapt<Order>();
            order.Code = code;
            order.UserId = userId;
            await _orderRepository.CreateAsync(order);

            foreach (var item in cart.CartItems)
            {
                item.Product.Stock -= item.Quantity;
                await _productRepository.UpdateStockAsync(item.ProductId, item.Product.Stock);
            }
            cart.CartItems.Clear();
            cart.Total = 0;
            cart.SubTotal = 0;
            await _cartRepository.UpdateAsync(cart);
            return code;
        }

        /// <summary>
        /// Obtiene una lista de órdenes para un usuario específico.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda</param>
        /// <param name="userId">Id del usuario al que pertenecen las órdenes</param>
        /// <returns>Ordenes del usuario</returns>
        public async Task<ListedOrderDetailDTO> GetByUserIdAsync(
            SearchParamsDTO searchParams,
            int userId
        )
        {
            var (orders, totalCount) = await _orderRepository.GetByUserIdAsync(
                searchParams,
                userId
            );
            var totalPages = (int)
                Math.Ceiling((double)totalCount / (searchParams.PageSize ?? _defaultPageSize));
            int currentPage = searchParams.PageNumber;
            int pageSize = searchParams.PageSize ?? _defaultPageSize;
            if (currentPage < 1 || currentPage > totalPages)
            {
                throw new ArgumentOutOfRangeException("El número de página está fuera de rango.");
            }
            var listedOrders = new ListedOrderDetailDTO
            {
                Orders = orders.Adapt<List<OrderDetailDTO>>(),
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = currentPage,
                PageSize = pageSize,
            };
            return listedOrders;
        }

        /// <summary>
        /// Obtiene los detalles de una orden.
        /// </summary>
        /// <param name="orderCode">Código de la orden</param>
        /// <returns>El detalle de la orden</returns>
        public async Task<OrderDetailDTO> GetDetailAsync(string orderCode)
        {
            Order? order =
                await _orderRepository.GetByCodeAsync(orderCode)
                ?? throw new KeyNotFoundException("Orden no encontrada");
            return order.Adapt<OrderDetailDTO>();
        }

        /// <summary>
        /// Genera un código único para la orden.
        /// </summary>
        /// <returns>Código de la orden</returns>
        private async Task<string> GenerateOrderCodeAsync()
        {
            string code;
            do
            {
                var timestamp = DateTime.UtcNow.ToString("yyMMddHHmmss");
                var random = Random.Shared.Next(100, 999);
                code = $"ORD-{timestamp}-{random}";
            } while (await _orderRepository.CodeExistsAsync(code));
            return code;
        }

        public async Task<ListedAdminOrdersDTO> GetFilteredForAdminAsync(
            AdminOrderSearchParamsDTO searchParams
        )
        {
            var (orders, totalCount) = await _orderRepository.GetFilteredForAdminAsync(
                searchParams
            );

            var totalPages = (int)
                Math.Ceiling((double)totalCount / (searchParams.PageSize ?? _defaultPageSize));
            if (
                totalCount > 0
                && (searchParams.PageNumber < 1 || searchParams.PageNumber > totalPages)
            )
            {
                throw new ArgumentOutOfRangeException("El número de página está fuera de rango.");
            }

            return new ListedAdminOrdersDTO
            {
                Orders = orders.Adapt<List<AdminOrderListDTO>>(),
                TotalCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = searchParams.PageNumber,
                PageSize = searchParams.PageSize ?? _defaultPageSize,
            };
        }

        public async Task<AdminOrderDetailDTO> GetByIdForAdminAsync(int orderId)
        {
            var order =
                await _orderRepository.GetByIdForAdminAsync(orderId)
                ?? throw new KeyNotFoundException($"Orden con ID {orderId} no encontrada.");

            return order.Adapt<AdminOrderDetailDTO>();
        }

        public async Task UpdateStatusAsync(int orderId, UpdateOrderStatusDTO dto, string adminId)
        {
            if (!Enum.TryParse<OrderStatus>(dto.Status, true, out var newStatus))
            {
                // Tu middleware lo convertirá en 400
                throw new ArgumentException("El estado proporcionado no es válido.");
            }

            var order = await _orderRepository.GetByIdForAdminAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException($"Orden con ID {orderId} no encontrada.");
            }

            bool isValidTransition = (order.Status, newStatus) switch
            {
                // Transiciones válidas
                (OrderStatus.Pending, OrderStatus.Paid) => true,
                (OrderStatus.Pending, OrderStatus.Cancelled) => true,
                (OrderStatus.Paid, OrderStatus.Shipped) => true,
                (OrderStatus.Paid, OrderStatus.Cancelled) => true,
                (OrderStatus.Shipped, OrderStatus.Delivered) => true,
                (OrderStatus.Shipped, OrderStatus.Cancelled) => true,

                _ when order.Status == newStatus => true,

                // Transiciones inválidas
                _ => false,
            };

            if (!isValidTransition)
            {
                // Tu middleware lo convertirá en 409 Conflict
                throw new InvalidOperationException(
                    $"Transición de estado no permitida de '{order.Status}' a '{newStatus}'."
                );
            }

            await _orderRepository.UpdateStatusAsync(orderId, newStatus, adminId);

            Log.Information(
                "Estado de la orden {OrderId} actualizado a {NewStatus} por Admin {AdminId}",
                orderId,
                newStatus,
                adminId
            );
        }
    }
}
