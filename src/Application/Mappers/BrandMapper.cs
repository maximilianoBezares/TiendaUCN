using Mapster;
using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Application.DTO.BrandDTO;

namespace TiendaUCN.src.Application.Mappers
{
    public class BrandMapper
    {
        /// <summary>
        /// Configuración de la aplicación.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor de la clase BrandMapper.
        /// </summary>
        public BrandMapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Configura todos los mapeos necesarios.
        /// </summary>
        public void ConfigureAllMappings()
        {
            ConfigureBrandMappings();
        }

        /// <summary>
        /// Configura los mapeos para la entidad Category.
        /// </summary>
        public void ConfigureBrandMappings()
        {
            TypeAdapterConfig<Brand, BrandDTO>.NewConfig()
                .Map(dest => dest.id, src => src.Id)
                .Map(dest => dest.name, src => src.Name)
                .Map(dest => dest.description, src => src.Description)
                .Map(dest => dest.slug, src => src.Slug);
        }
    }
}