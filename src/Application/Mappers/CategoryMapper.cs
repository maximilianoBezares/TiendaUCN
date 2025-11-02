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
                .Map(dest => dest.name, src => src.Name)
                .Map(dest => dest.description, src => src.Description)
                .Map(dest => dest.slug, src => src.Slug);

            TypeAdapterConfig<Category, CategoryDetailDTO>.NewConfig()
                .Map(dest => dest.id, src => src.Id)
                .Map(dest => dest.name, src => src.Name)
                .Map(dest => dest.description, src => src.Description)
                .Map(dest => dest.createdAt, src => src.CreatedAt)
                .Map(dest => dest.slug, src => src.Slug);

            TypeAdapterConfig<CategoryCreateDTO, Category>.NewConfig()
                .Map(dest => dest.Name, src => src.name)
                .Map(dest => dest.Description, src => src.description);

            TypeAdapterConfig<CategoryUpdateDTO, Category>.NewConfig()
                .Map(dest => dest.Name, src => src.name)
                .Map(dest => dest.Description, src => src.description);
        }
    }
}
