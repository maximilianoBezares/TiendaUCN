using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.src.Application.DTO.ProductDTO;
using TiendaUCN.src.Application.DTO.CategoriesDTO;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.Infrastructure.Repositories.Interfaces
{
    public interface ICategoriesRepository
    {
        Task<(IEnumerable<Category> categories, int totalCount)> GetAllCategoriesAsync(SearchParamsDTO searchParams);
        Task<Category?> GetByIdAdminAsync(int id);
        Task<int> GetProductCountByIdAsync(int id);
        Task<Category?> GetByNameAsync(string name);
        Task<int> CreateAsync(Category category);
    }
}