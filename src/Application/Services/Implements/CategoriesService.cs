using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.src.Application.DTO.CategoriesDTO;
using TiendaUCN.src.Application.DTO.ProductDTO;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Infrastructure.Repositories.Interfaces;
using Mapster;

namespace TiendaUCN.src.Application.Services.Implements
{
    public class CategoriesService : ICategoriesService
    {
        /// <summary>
        /// Repositorio de categorías.
        /// </summary>
        private readonly ICategoriesRepository _categoriesRepository;

        /// <summary>
        /// Tamaño de página predeterminado para la paginación.
        /// </summary>
        private readonly int _defaultPageSize;

        /// <summary>
        /// Configuración de la aplicación.
        /// </summary>
        private readonly IConfiguration _configuration;

        public CategoriesService(ICategoriesRepository categoriesRepository, IConfiguration configuration)
        {
            _categoriesRepository = categoriesRepository;
            _configuration = configuration;
            _defaultPageSize = int.Parse(_configuration["Categories:DefaultPageSize"] ?? throw new InvalidOperationException("La configuración 'DefaultPageSize' no está definida."));
        }

        /// <summary>
        /// Obtiene todas las categorías con paginación y búsqueda.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda y paginación.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el listado de categorías.</returns>
        public async Task<ListedCategoriesDTO> GetCategoriesForAdminAsync(SearchParamsDTO searchParams)
        {
            Log.Information("Obteniendo todas las categorías con parámetros de búsqueda: {@SearchParams}", searchParams);
            var (categories1, totalCount) = await _categoriesRepository.GetAllCategoriesAsync(searchParams);
            var totalPages = (int)Math.Ceiling((double)totalCount / (searchParams.PageSize ?? _defaultPageSize));
            int currentPage = searchParams.PageNumber;
            int pageSize = searchParams.PageSize ?? _defaultPageSize;
            if (currentPage < 1 || currentPage > totalPages)
            {
                throw new ArgumentOutOfRangeException("El número de página está fuera de rango.");
            }
            Log.Information("Total de categorías encontradas: {TotalCount}, Total de páginas: {TotalPages}, Página actual: {CurrentPage}, Tamaño de página: {PageSize}", totalCount, totalPages, currentPage, pageSize);
            return new ListedCategoriesDTO
            {
                categories = categories1.Adapt<List<CategoryDTO>>(),
                totalCount = totalCount,
                totalPages = totalPages,
                currentPage = currentPage,
                pageSize = categories1.Count()
            };
        }
   
        /// <summary>
        /// Retorna una categoria específica por su ID desde el punto de vista de un admin.
        /// </summary>
        /// <param name="id">El ID de la categoria a buscar.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el producto encontrado o null si no se encuentra.</returns>
        public async Task<CategoryDetailDTO> GetCategoryByIdForAdminAsync(int id)
        {
            var category = await _categoriesRepository.GetByIdAdminAsync(id) ?? throw new KeyNotFoundException($"Categoria con id {id} no encontrado.");
            Log.Information("Categoria encontrada: {@Category}", category);
            var dto = category.Adapt<CategoryDetailDTO>();
            dto.productCount = await _categoriesRepository.GetProductCountByIdAsync(category.Id);
            return dto;
        }
    }
}