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
using TiendaUCN.src.Domain.Models;

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


        /// <summary>
        /// Obtiene una categoría específica por su ID para admin.
        /// </summary>
        public async Task<BrandDetailDTO> GetBrandByIdForAdminAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAdminAsync(id) ?? throw new KeyNotFoundException($"Marca con id {id} no encontrado.");
            Log.Information("Categoria encontrada: {@Category}", brand);
            var dto = brand.Adapt<BrandDetailDTO>();
            dto.productCount = await _brandRepository.GetProductCountByIdAsync(brand.Id);
            return dto;
        }

        /// <summary>
        /// Crea una nueva marca en el sistema
        /// </summary>
        public async Task<string> CreateBrandAsync(BrandCreateDTO brandCreate)
        {
            Brand brand = brandCreate.Adapt<Brand>();
            var brandName = await _brandRepository.GetByNameAsync(brand.Name);
            if (brandName != null)
            {
                throw new InvalidOperationException($"Ya existe una marca con el nombre '{brand.Name}'.");
            }
            string slug = GenerateSlug(brand.Name);
            if (await _brandRepository.ExistsSlug(slug))
            {
                throw new InvalidOperationException($"El nombre de marca genera un slug: {slug} que ya esta en uso, porfavor cambiar nombre");
            }
            brand.Slug = slug;
            brand.Name = Sanitize(brand.Name);
            brand.Description = Sanitize(brand.Description);
            int brandId = await _brandRepository.CreateAsync(brand);
            Log.Information("Marca creada: {@Brand}", brand);
            return brandId.ToString();
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

        /// <summary>
        /// Metodo privado para sanitizar los atributos o inputs que se pongan al crear o actualizar una marca
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

        /// <summary>
        /// Actualiza una marca en el sistema
        /// </summary>
        public async Task<BrandUpdateDTO> UpdateBrandAsync(int id, BrandUpdateDTO brandUpdate)
        {
            var brand1 = await _brandRepository.GetByIdAdminAsync(id);
            if (brand1 == null)
            {
                throw new KeyNotFoundException($"No se encontró la marca con ID {id}.");
            }
            var brandName = await _brandRepository.GetByNameAsync(brandUpdate.name);
            if (brandName != null && brandName.Id != id)
            {
                throw new InvalidOperationException($"Ya existe otra marca con el nombre '{brandUpdate.name}'.");
            }
            var slug = GenerateSlug(brandUpdate.name);
            if (await _brandRepository.ExistsSlug(slug))
            {
                throw new InvalidOperationException($"El nombre de marca genera un slug: {slug} que ya esta en uso, porfavor cambiar nombre");
            }
            brandUpdate.Adapt(brand1);
            brand1.Name = Sanitize(brandUpdate.name);
            brand1.Description = Sanitize(brandUpdate.description);
            brand1.Slug = slug;
            await _brandRepository.UpdateAsync(brand1);
            Log.Information("Marca actualizada con id", id);
            return brandUpdate;
        }

        /// <summary>
        /// Elimina una marca del sistema.
        /// </summary>
        public async Task DeleteBrandAsync(int id)
        {
            var brand1 = await _brandRepository.GetByIdAdminAsync(id);
            if (brand1 == null)
            {
                throw new KeyNotFoundException($"No se encontró la marca con ID {id} para eliminar.");
            }
            var productCount = await _brandRepository.GetProductCountByIdAsync(id);
            if (productCount > 0)
            {
                throw new InvalidOperationException($"No se puede eliminar la marca '{brand1.Name}' (ID: {id}) porque tiene {productCount} productos asociados. Primero debe desvincular los productos.");
            }
            await _brandRepository.DeleteAsync(id);
            Log.Information("Marca eliminada", id);
        }
    }
}