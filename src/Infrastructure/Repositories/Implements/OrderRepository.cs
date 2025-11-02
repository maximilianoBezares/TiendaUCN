using Microsoft.EntityFrameworkCore;
using TiendaUCN.src.Application.DTO.OrderDTO.AdminDTO;
using TiendaUCN.src.Application.DTO.ProductDTO;
using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Infrastructure.Data;
using TiendaUCN.src.Infrastructure.Repositories.Interfaces;

namespace TiendaUCN.src.Infrastructure.Repositories.Implements
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly int _defaultPageSize;

        public OrderRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _defaultPageSize = int.Parse(
                _configuration["Products:DefaultPageSize"]
                    ?? throw new InvalidOperationException(
                        "La configuración 'DefaultPageSize' no está definida."
                    )
            );
        }

        /// <summary>
        /// Verifica si un código de orden existe.
        /// </summary>
        /// <param name="orderCode">Código de la orden</param>
        /// <returns>True si el código existe, de lo contrario false.</returns>
        public async Task<bool> CodeExistsAsync(string orderCode)
        {
            return await _context.Orders.AnyAsync(o => o.Code == orderCode);
        }

        /// <summary>
        /// Crea una nueva orden.
        /// </summary>
        /// <param name="order">La orden a crear.</param>
        /// <returns>Booleano que indica si la creación fue exitosa.</returns>
        public async Task<bool> CreateAsync(Order order)
        {
            Order? existingOrder = await _context.Orders.FirstOrDefaultAsync(o =>
                o.Code == order.Code
            );
            if (existingOrder != null)
            {
                return false;
            }
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Obtiene una orden por su código.
        /// </summary>
        /// <param name="orderCode">El código de la orden.</param>
        /// <returns>La orden correspondiente o null si no se encuentra.</returns>
        public async Task<Order?> GetByCodeAsync(string orderCode)
        {
            return await _context
                .Orders.AsNoTracking()
                .Include(or => or.OrderItems)
                .FirstOrDefaultAsync(o => o.Code == orderCode);
        }

        /// <summary>
        /// Obtiene las órdenes de un usuario por su ID.
        /// </summary>
        /// <param name="userId">El ID del usuario.</param>
        /// <param name="searchParams">Los parámetros de búsqueda.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con una lista de órdenes y el conteo total de órdenes.</returns>
        public async Task<(IEnumerable<Order> orders, int totalCount)> GetByUserIdAsync(
            SearchParamsDTO searchParams,
            int userId
        )
        {
            var query = _context
                .Orders.AsNoTracking()
                .Include(or => or.OrderItems)
                .Where(or => or.UserId == userId);
            var totalCount = await query.CountAsync();
            string searchTerm = searchParams.SearchTerm?.Trim().ToLower() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                query = query.Where(or =>
                    or.Code.Contains(searchTerm)
                    || or.OrderItems.Any(oi =>
                        oi.TitleAtMoment.ToLower().Contains(searchTerm)
                        || or.OrderItems.Any(oi =>
                            oi.DescriptionAtMoment.ToLower().Contains(searchTerm)
                        )
                    )
                );
            }
            query = query
                .Skip((searchParams.PageNumber - 1) * searchParams.PageSize ?? _defaultPageSize)
                .Take(searchParams.PageSize ?? _defaultPageSize);
            return (await query.ToListAsync(), totalCount);
        }

        public async Task<Order?> GetByIdForAdminAsync(int orderId)
        {
            return await _context
                .Orders.Include(o => o.User)
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task UpdateStatusAsync(int orderId, OrderStatus newStatus, string adminId)
        {
            await _context
                .Orders.Where(o => o.Id == orderId)
                .ExecuteUpdateAsync(s =>
                    s.SetProperty(o => o.Status, newStatus)
                        .SetProperty(o => o.UpdatedAt, DateTime.UtcNow)
                        .SetProperty(o => o.UpdatedByAdminId, adminId)
                );
        }

        public async Task<(IEnumerable<Order> orders, int totalCount)> GetFilteredForAdminAsync(
            AdminOrderSearchParamsDTO searchParams
        )
        {
            var query = _context
                .Orders.Include(o => o.User)
                .Include(o => o.OrderItems)
                .AsNoTracking();

            if (searchParams.Status.HasValue)
            {
                query = query.Where(o => o.Status == searchParams.Status.Value);
            }
            if (searchParams.From.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= searchParams.From.Value.ToUniversalTime());
            }
            if (searchParams.To.HasValue)
            {
                query = query.Where(o => o.CreatedAt <= searchParams.To.Value.ToUniversalTime());
            }
            if (!string.IsNullOrWhiteSpace(searchParams.CustomerEmail))
            {
                query = query.Where(o =>
                    o.User.Email.ToLower().Contains(searchParams.CustomerEmail.ToLower())
                );
            }
            if (!string.IsNullOrWhiteSpace(searchParams.OrderCode))
            {
                query = query.Where(o =>
                    o.Code.ToLower().Contains(searchParams.OrderCode.ToLower())
                );
            }

            var sortBy = searchParams.SortBy ?? "createdAt_desc";
            query = sortBy switch
            {
                "total_asc" => query.OrderBy(o => o.Total),
                "total_desc" => query.OrderByDescending(o => o.Total),
                "createdAt_asc" => query.OrderBy(o => o.CreatedAt),
                _ => query.OrderByDescending(o => o.CreatedAt), // Default
            };

            int totalCount = await query.CountAsync();

            var orders = await query
                .Skip((searchParams.PageNumber - 1) * (searchParams.PageSize ?? _defaultPageSize))
                .Take(searchParams.PageSize ?? _defaultPageSize)
                .ToListAsync();

            return (orders, totalCount);
        }
    }
}
