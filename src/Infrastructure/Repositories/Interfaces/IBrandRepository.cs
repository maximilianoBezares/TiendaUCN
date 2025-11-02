using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.src.Application.DTO.ProductDTO;
using TiendaUCN.src.Application.DTO.BrandDTO;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.Infrastructure.Repositories.Interfaces
{
    public interface IBrandRepository
    {
        Task<(IEnumerable<Brand> brands, int totalCount)> GetAllBrandsAsync(SearchParamsDTO searchParams);
        Task<int> GetProductCountByIdAsync(int id);
        Task<Brand?> GetByIdAdminAsync(int id);
        Task<Brand?> GetByNameAsync(string name);
        Task<int> CreateAsync(Brand brand);
        Task<bool> ExistsSlug(string slug);
    }
}