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
using TiendaUCN.src.Domain.Models;

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
            var categoriesDto = categories1.Adapt<List<CategoryDTO>>();
            foreach (var dto in categoriesDto)
            {
                dto.productCount = await _categoriesRepository.GetProductCountByIdAsync(dto.id);
            }
            Log.Information("Total de categorías encontradas: {TotalCount}, Total de páginas: {TotalPages}, Página actual: {CurrentPage}, Tamaño de página: {PageSize}", totalCount, totalPages, currentPage, pageSize);
            return new ListedCategoriesDTO
            {
                categories = categoriesDto,
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

        /// <summary>
        /// Crea una nueva categoria en el sistema
        /// </summary>
        /// <param name="categoryCreate"> datos de la categoria a crear</param>
        public async Task<string> CreateCategoryAsync(CategoryCreateDTO categoryCreate)
        {
            Category category = categoryCreate.Adapt<Category>();
            var categoryName = await _categoriesRepository.GetByNameAsync(category.Name);
            if (categoryName != null)
            {
                throw new InvalidOperationException($"Ya existe una categoría con el nombre '{category.Name}'.");
            }
            string slug = GenerateSlug(category.Name);
            if (await _categoriesRepository.ExistsSlug(slug))
            {
                throw new InvalidOperationException($"El nombre de categoría genera un slug: {slug} que ya esta en uso, porfavor cambiar nombre");
            }
            category.Slug = slug;
            category.Name = Sanitize(category.Name);
            category.Description = Sanitize(category.Description);
            int categoryId = await _categoriesRepository.CreateAsync(category);
            Log.Information("Categoría creada: {@Category}", category);
            return categoryId.ToString();
        }

        /// <summary>
        /// Genera un slug a partir del nombre
        /// </summary>
        private string GenerateSlug(string name)
        {
            string slug = name.ToLowerInvariant();
            slug = slug
                    .Replace("á", "a")
                    .Replace("é", "e")
                    .Replace("í", "i")
                    .Replace("ó", "o")
                    .Replace("ú", "u")
                    .Replace("ñ", "n");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = slug.Replace(" ", "-");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');
            return slug;
        }

        public async Task<CategoryUpdateDTO> UpdateCategoryAsync(int id, CategoryUpdateDTO categoryUpdate)
        {
            var category1 = await _categoriesRepository.GetByIdAdminAsync(id);
            if (category1 == null)
            {
                throw new KeyNotFoundException($"No se encontró la categoría con ID {id}.");
            }
            var categoryName = await _categoriesRepository.GetByNameAsync(categoryUpdate.name);
            if (categoryName != null && categoryName.Id != id)
            {
                throw new InvalidOperationException($"Ya existe otra categoría con el nombre '{categoryUpdate.name}'.");
            }
            var slug = GenerateSlug(categoryUpdate.name);
            if (await _categoriesRepository.ExistsSlug(slug))
            {
                throw new InvalidOperationException($"El nombre de categoría genera un slug: {slug} que ya esta en uso, porfavor cambiar nombre");
            }
            categoryUpdate.Adapt(category1);
            category1.Name = Sanitize(categoryUpdate.name);
            category1.Description = Sanitize(categoryUpdate.description);
            category1.Slug = slug;
            await _categoriesRepository.UpdateAsync(category1);
            Log.Information("Categoría actualizada con id", id);
            return categoryUpdate;
        }

        /// <summary>
        /// Metodo privado para sanitizar los atributos o inputs que se pongan al crear o actualizar una categoria
        /// </summary>
        private string Sanitize(string attribute)
        {
            if (string.IsNullOrWhiteSpace(attribute))
            {
                return attribute ?? string.Empty;
            }
            string sanitized = System.Text.RegularExpressions.Regex.Replace(attribute, "<.*?>", string.Empty);
            return sanitized.Trim();
        }
    }
}