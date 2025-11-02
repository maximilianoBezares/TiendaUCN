using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.src.Application.DTO.BrandDTO;
using TiendaUCN.src.Application.DTO.ProductDTO;

namespace TiendaUCN.src.Application.Services.Interfaces
{
    public interface IBrandService
    {
        /// <summary>
        /// Obtiene todas las marcas con paginación y búsqueda para admin.
        /// </summary>
        Task<ListedBrandsDTO> GetBrandsForAdminAsync(SearchParamsDTO searchParams);

        /// <summary>
        /// Obtiene una categoría específica por su ID para admin.
        /// </summary>
        Task<BrandDetailDTO> GetBrandByIdForAdminAsync(int id);


        /// <summary>
        /// Crea una nueva Marca en el sistema
        /// </summary>
        Task<string> CreateBrandAsync(BrandCreateDTO brandCreate);

        /// <summary>
        /// Actualiza una marca en el sistema
        /// </summary>
        Task<BrandUpdateDTO> UpdateBrandAsync(int id, BrandUpdateDTO brandUpdate);
    }
}