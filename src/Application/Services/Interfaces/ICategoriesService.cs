using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.src.Application.DTO.CategoriesDTO;
using TiendaUCN.src.Application.DTO.ProductDTO;

namespace TiendaUCN.src.Application.Services.Interfaces
{
    public interface ICategoriesService
    {
        /// <summary>
        /// Obtiene todas las categorías con paginación y búsqueda para admin.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda y paginación.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el listado de categorías.</returns>
        Task<ListedCategoriesDTO> GetCategoriesForAdminAsync(SearchParamsDTO searchParams);

        /// <summary>
        /// Obtiene una categoría específica por su ID para admin.
        /// </summary>
        /// <param name="id">ID de la categoría.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el detalle de la categoría.</returns>
        Task<CategoryDetailDTO> GetCategoryByIdForAdminAsync(int id);

        /// <summary>
        /// Crea una nueva categoria en el sistema
        /// </summary>
        /// <param name="categoryCreate"> datos de la categoria a crear</param>
        Task<string> CreateCategoryAsync(CategoryCreateDTO categoryCreate);
    }
}