using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.src.Infrastructure.Data;
using TiendaUCN.src.Infrastructure.Repositories.Interfaces;
using TiendaUCN.src.Application.DTO.ProductDTO;
using TiendaUCN.src.Application.DTO.CategoriesDTO;
using TiendaUCN.src.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace TiendaUCN.src.Infrastructure.Repositories.Implements
{
    public class CategoriesRepository : ICategoriesRepository
    {
        /// <summary>
        /// Contexto de la base de datos.
        /// </summary>
        private readonly DataContext _context;

        /// <summary>
        /// Tamaño de página predeterminado para la paginación.
        /// </summary>
        private readonly int _defaultPageSize;

        /// <summary>
        /// Configuración de la aplicación.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor del repositorio de categorías.
        /// </summary>
        public CategoriesRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _defaultPageSize = _configuration.GetValue<int?>("Categories:DefaultPageSize") ?? throw new ArgumentNullException("El tamaño de página por defecto no puede ser nulo.");
        }

        /// <summary>
        /// Obtiene todas las categorías con paginación y búsqueda.
        /// </summary>
        public async Task<(IEnumerable<Category> categories, int totalCount)> GetAllCategoriesAsync(SearchParamsDTO searchParams)
        {
            var query = _context.Categories.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(searchTerm));
            }
            int totalCount = await query.CountAsync();
            var categories1 = await query
                .OrderBy(c => c.Name)
                .Skip((searchParams.PageNumber - 1) * (searchParams.PageSize ?? _defaultPageSize))
                .Take(searchParams.PageSize ?? _defaultPageSize)
                .ToListAsync();
            return (categories1, totalCount);
        }

        /// <summary>
        /// Obtienne las categorias mediante el id 
        /// </summary>
        public async Task<Category?> GetByIdAdminAsync(int id)
        {
            return await _context.Categories.AsNoTracking().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Obtiende la cantidad de productos que hay en una categoria.
        /// </summary>
        public async Task<int> GetProductCountByIdAsync(int id)
        {
            return await _context.Products.CountAsync(p => p.CategoryId == id);
        }

        /// <summary>
        /// Obtiene las categorias mediante el nombre.
        /// </summary>
        public async Task<Category?> GetByNameAsync(string name)
        {
            return await _context.Categories.AsNoTracking().Where(c => c.Name == name).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Crea una categoria en el sistema.
        /// </summary>
        public async Task<int> CreateAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return category.Id;
        }
    }
}