using Mapster;

namespace TiendaUCN.src.Application.Mappers
{
    /// <summary>
    /// Clase para extensiones de mapeo.
    /// Contiene configuraciones globales de mapeo.
    /// </summary>
    public class MapperExtensions
    {
        /// <summary>
        /// Configura los mapeos globales.
        /// </summary>
        public static void ConfigureMapster(IServiceProvider serviceProvider)
        {
            UserMapper.ConfigureAllMappings();

            var productMapper = serviceProvider.GetService<ProductMapper>();
            productMapper?.ConfigureAllMappings();

            var cartMapper = serviceProvider.GetService<CartMapper>();
            cartMapper?.ConfigureAllMappings();

            var orderMapper = serviceProvider.GetService<OrderMapper>();
            orderMapper?.ConfigureAllMappings();;

            // Configuración global de Mapster para ignorar valores nulos
            TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);
        }
    }
}
