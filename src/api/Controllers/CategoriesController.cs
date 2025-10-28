using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Application.DTO.ProductDTO;
using TiendaUCN.src.Application.DTO.CategoriesDTO;
using TiendaUCN.src.Application.DTO;

namespace TiendaUCN.src.api.Controllers
{
    /// <summary>
    /// Controller para gestionar las categorías de productos.
    /// </summary>
    public class CategoriesController : BaseController
    {
        /// <summary>
        /// Servicio de categorías.
        /// </summary>
        private readonly ICategoriesService _categoriesService;

        /// <summary>
        /// Constructor del controlador de categorías.
        /// </summary>
        public CategoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        /// <summary>
        /// Obtiene todas las categorías de productos para administradores con paginación y búsqueda.
        /// </summary>
        [HttpGet("admin/categories")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCategoriesForAdminAsync([FromQuery] SearchParamsDTO searchParams)
        {
            var result = await _categoriesService.GetCategoriesForAdminAsync(searchParams);
            if (result == null || result.categories.Count == 0) { throw new KeyNotFoundException("No se encontraron categorías."); }
            return Ok(new GenericResponse<ListedCategoriesDTO>("Categorías obtenidas exitosamente", result));
        }

        /// <summary>
        /// Obtiene una categoría específica por su ID para administradores.
        /// </summary>
        [HttpGet("admin/categories/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCategoryByIdForAdminAsync(int id)
        {
            var result = await _categoriesService.GetCategoryByIdForAdminAsync(id);
            if (result == null) { throw new KeyNotFoundException($"No se encontró la categoría con ID {id}."); }
            return Ok(new GenericResponse<CategoryDetailDTO>("Categoría obtenida exitosamente", result));
        }
    }
}