using Mapster;
using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Application.DTO.CategoriesDTO;

namespace TiendaUCN.src.Application.Mappers
{
    public class CategoryMapper
    {
        /// <summary>
        /// Configuración de la aplicación.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor de la clase CategoryMapper.
        /// </summary>
        public CategoryMapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Configura todos los mapeos necesarios.
        /// </summary>
        public void ConfigureAllMappings()
        {
            ConfigureCategoryMappings();
        }

        /// <summary>
        /// Configura los mapeos para la entidad Category.
        /// </summary>
        public void ConfigureCategoryMappings()
        {
            TypeAdapterConfig<Category, CategoryDTO>.NewConfig()
                .Map(dest => dest.id, src => src.Id)
                .Map(dest => dest.name, src => src.Name);
        }
    }
}
