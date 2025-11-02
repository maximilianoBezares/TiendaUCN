using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Application.DTO.BrandDTO;
using TiendaUCN.src.Application.DTO.ProductDTO;
using TiendaUCN.src.Infrastructure.Repositories.Interfaces;
using Serilog;
using Mapster;

using TiendaUCN.src.Infrastructure.Repositories.Implements;

namespace TiendaUCN.src.Application.Services.Implements
{
    public class BrandService : IBrandService
    {
        /// <summary>
        /// Repositorio de marcas.
        /// </summary>
        private readonly IBrandRepository _brandRepository;

        /// <summary>
        /// Tamaño de página predeterminado para la paginación.
        /// </summary>
        private readonly int _defaultPageSize;

        /// <summary>
        /// Configuración de la aplicación.
        /// </summary>
        private readonly IConfiguration _configuration;

        public BrandService(IBrandRepository brandRepository, IConfiguration configuration)
        {
            _brandRepository = brandRepository;
            _configuration = configuration;
            _defaultPageSize = int.Parse(_configuration["Brands:DefaultPageSize"] ?? throw new InvalidOperationException("La configuración 'DefaultPageSize' no está definida."));
        }

        /// <summary>
        /// Obtiene todas las marcas con paginación y búsqueda para admin.
        /// </summary>
        public async Task<ListedBrandsDTO> GetBrandsForAdminAsync(SearchParamsDTO searchParams)
        {
            Log.Information("Obteniendo todas las marcas con parámetros de búsqueda: {@SearchParams}", searchParams);
            var (brands1, totalCount) = await _brandRepository.GetAllBrandsAsync(searchParams);
            var totalPages = (int)Math.Ceiling((double)totalCount / (searchParams.PageSize ?? _defaultPageSize));
            int currentPage = searchParams.PageNumber;
            int pageSize = searchParams.PageSize ?? _defaultPageSize;
            if (currentPage < 1 || currentPage > totalPages)
            {
                throw new ArgumentOutOfRangeException("El número de página está fuera de rango.");
            }
            var brandsDto = brands1.Adapt<List<BrandDTO>>();
            foreach (var dto in brandsDto)
            {
                dto.productCount = await _brandRepository.GetProductCountByIdAsync(dto.id);
            }
            Log.Information("Total de categorías encontradas: {TotalCount}, Total de páginas: {TotalPages}, Página actual: {CurrentPage}, Tamaño de página: {PageSize}", totalCount, totalPages, currentPage, pageSize);
            return new ListedBrandsDTO
            {
                brands = brandsDto,
                totalCount = totalCount,
                totalPages = totalPages,
                currentPage = currentPage,
                pageSize = brands1.Count()
            };
        }
    }
}