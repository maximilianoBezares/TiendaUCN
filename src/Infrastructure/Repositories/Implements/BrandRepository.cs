using TiendaUCN.src.Infrastructure.Data;
using TiendaUCN.src.Infrastructure.Repositories.Interfaces;
using TiendaUCN.src.Application.DTO.ProductDTO;
using TiendaUCN.src.Application.DTO.BrandDTO;
using TiendaUCN.src.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace TiendaUCN.src.Infrastructure.Repositories.Implements
{
    public class BrandRepository : IBrandRepository
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
        /// Constructor del repositorio de marcas.
        /// </summary>
        public BrandRepository(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _defaultPageSize = _configuration.GetValue<int?>("Brands:DefaultPageSize") ?? throw new ArgumentNullException("El tamaño de página por defecto no puede ser nulo.");
        }

        public async Task<(IEnumerable<Brand> brands, int totalCount)> GetAllBrandsAsync(SearchParamsDTO searchParams)
        {
            var query = _context.Brands.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(searchParams.SearchTerm))
            {
                var searchTerm = searchParams.SearchTerm.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(searchTerm));
            }
            int totalCount = await query.CountAsync();
            var brands1 = await query
                .OrderBy(c => c.Name)
                .Skip((searchParams.PageNumber - 1) * (searchParams.PageSize ?? _defaultPageSize))
                .Take(searchParams.PageSize ?? _defaultPageSize)
                .ToListAsync();
            return (brands1, totalCount);
        }

        /// <summary>
        /// Obtiende la cantidad de productos que hay en una marca.
        /// </summary>
        public async Task<int> GetProductCountByIdAsync(int id)
        {
            return await _context.Products.CountAsync(p => p.BrandId == id);
        }

        /// <summary>
        /// Obtiene las marcas mediante el id
        /// </summary>
        public async Task<Brand?> GetByIdAdminAsync(int id)
        {
            return await _context.Brands.AsNoTracking().Where(b => b.Id == id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Obtiene las marcas mediante el nombre
        /// </summary>
        public async Task<Brand?> GetByNameAsync(string name)
        {
            return await _context.Brands.AsNoTracking().Where(b => b.Name == name).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Crea una marca en el sistema.
        /// </summary>
        public async Task<int> CreateAsync(Brand brand)
        {
            await _context.Brands.AddAsync(brand);
            await _context.SaveChangesAsync();
            return brand.Id;
        }

        /// <summary>
        /// Compara si existe un mismo slug en la base de datos con el pasado en parametro, si devuelve true, entonces los dos slugs son iguales, si devuelve false, no son iguales
        /// </summary>
        public async Task<bool> ExistsSlug(string slug)
        {
            return await _context.Brands.AsNoTracking().Where(b => b.Slug.ToLower() == slug.ToLower()).AnyAsync();
        }

        /// <summary>
        /// Actualiza una marca en el sistema.
        /// </summary>
        public async Task UpdateAsync(Brand brand)
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }
    }
}