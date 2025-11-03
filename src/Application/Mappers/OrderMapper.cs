using Mapster;
using TiendaUCN.src.Application.DTO.CartDTO;
using TiendaUCN.src.Application.DTO.OrderDTO;
using TiendaUCN.src.Application.DTO.OrderDTO.AdminDTO;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.Application.Mappers
{
    /// <summary>
    /// Clase para mapeo de órdenes a DTO y viceversa.
    /// </summary>
    public class OrderMapper
    {
        private readonly IConfiguration _configuration;
        private readonly string _defaultImageURL;
        private readonly TimeZoneInfo _timeZone;

        public OrderMapper(IConfiguration configuration)
        {
            _configuration = configuration;
            _defaultImageURL =
                _configuration["Products:DefaultImageUrl"]
                ?? throw new InvalidOperationException(
                    "La configuración de DefaultImageUrl es necesaria."
                );
            _timeZone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneInfo.Local.Id);
        }

        public void ConfigureAllMappings()
        {
            ConfigureOrderItemsMappings();
            ConfigureOrderMappings();
            ConfigureAdminOrderMappings();
        }

        public void ConfigureOrderMappings()
        {
            TypeAdapterConfig<Order, OrderDetailDTO>
                .NewConfig()
                .Map(dest => dest.Items, src => src.OrderItems)
                .Map(
                    dest => dest.PurchasedAt,
                    src => TimeZoneInfo.ConvertTimeFromUtc(src.CreatedAt, _timeZone)
                )
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.Total, src => src.Total.ToString("C"))
                .Map(dest => dest.SubTotal, src => src.SubTotal.ToString("C"))
                .Map(dest => dest.Savings, src => (src.SubTotal - src.Total).ToString("C"));

            TypeAdapterConfig<Cart, Order>
                .NewConfig()
                .Map(dest => dest.Total, src => src.Total)
                .Map(dest => dest.SubTotal, src => src.SubTotal)
                .Map(dest => dest.OrderItems, src => src.CartItems)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.Code)
                .Ignore(dest => dest.CreatedAt);
        }

        public void ConfigureOrderItemsMappings()
        {
            TypeAdapterConfig<OrderItem, OrderItemDTO>
                .NewConfig()
                .Map(dest => dest.ProductTitle, src => src.TitleAtMoment)
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(dest => dest.ProductDescription, src => src.DescriptionAtMoment)
                .Map(dest => dest.MainImageURL, src => src.ImageAtMoment)
                .Map(dest => dest.PriceAtMoment, src => src.PriceAtMoment.ToString("C"));

            TypeAdapterConfig<CartItem, OrderItem>
                .NewConfig()
                .Map(dest => dest.TitleAtMoment, src => src.Product.Title)
                .Map(dest => dest.Quantity, src => src.Quantity)
                .Map(dest => dest.DescriptionAtMoment, src => src.Product.Description)
                .Map(
                    dest => dest.ImageAtMoment,
                    src =>
                        src.Product.Images != null && src.Product.Images.Any()
                            ? src.Product.Images.First().ImageUrl
                            : _defaultImageURL
                )
                .Map(dest => dest.PriceAtMoment, src => src.Product.Price)
                .Map(dest => dest.DiscountAtMoment, src => src.Product.Discount)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.OrderId)
                .Ignore(dest => dest.Order);
        }

        public void ConfigureAdminOrderMappings()
        {
            TypeAdapterConfig<Order, AdminOrderListDTO>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Code, src => src.Code)
                .Map(
                    dest => dest.CreatedAt,
                    src => TimeZoneInfo.ConvertTimeFromUtc(src.CreatedAt, _timeZone)
                )
                .Map(dest => dest.Status, src => src.Status.ToString())
                .Map(dest => dest.CustomerEmail, src => src.User.Email) // Requiere .Include(o => o.User)
                .Map(dest => dest.CustomerName, src => src.User.UserName) // Asumiendo que UserName es el nombre
                .Map(dest => dest.Total, src => src.Total.ToString("C"))
                .Map(dest => dest.ItemsCount, src => src.OrderItems.Count); // Requiere .Include(o => o.OrderItems)

            TypeAdapterConfig<Order, AdminOrderDetailDTO>
                .NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Code, src => src.Code)
                .Map(dest => dest.Status, src => src.Status.ToString())
                .Map(
                    dest => dest.CreatedAt,
                    src => TimeZoneInfo.ConvertTimeFromUtc(src.CreatedAt, _timeZone)
                )
                .Map(
                    dest => dest.UpdatedAt,
                    src => TimeZoneInfo.ConvertTimeFromUtc(src.UpdatedAt, _timeZone)
                )
                .Map(dest => dest.Total, src => src.Total.ToString("C"))
                .Map(dest => dest.SubTotal, src => src.SubTotal.ToString("C"))
                .Map(dest => dest.Savings, src => (src.SubTotal - src.Total).ToString("C"))
                .Map(
                    dest => dest.Customer,
                    src => new CustomerInfoDTO
                    {
                        UserId = src.UserId,
                        Email = src.User.Email,
                        FullName = src.User.UserName,
                    }
                ) // Requiere .Include(o => o.User)
                .Map(dest => dest.Items, src => src.OrderItems); // Requiere .Include(o => o.OrderItems)
        }
    }
}
