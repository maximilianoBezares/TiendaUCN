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
    }
}