using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TiendaUCN.src.Application.Services.Interfaces;
using TiendaUCN.src.Application.DTO.ProductDTO;
using TiendaUCN.src.Application.DTO.BrandDTO;
using TiendaUCN.src.Application.DTO;

namespace TiendaUCN.src.api.Controllers
{
    /// <summary>
    /// Controlador para el manejo de marcas para admin
    /// </summary>
    public class BrandController : BaseController
    {
        /// <summary>
        /// Servicio de marcas
        /// </summary>
        private readonly IBrandService _brandService;

        /// <summary>
        /// Constructor para la clase marca
        /// </summary>
        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        /// <summary>
        /// Obtiene todas las marcas para administradores con paginación y búsqueda.
        /// </summary>
        [HttpGet("admin/brands")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBrandForAdminAsync([FromQuery] SearchParamsDTO searchParams)
        {
            var result = await _brandService.GetBrandsForAdminAsync(searchParams);
            if (result == null || result.brands.Count == 0) { throw new KeyNotFoundException("No se encontraron marcas."); }
            return Ok(new GenericResponse<ListedBrandsDTO>("Marcas obtenidas exitosamente", result));
        }

        /// <summary>
        /// Obtiene una marca específica por su ID para administradores.
        /// </summary>
        [HttpGet("admin/brands/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBrandByIdForAdminAsync(int id)
        {
            var result = await _brandService.GetBrandByIdForAdminAsync(id);
            if (result == null) { throw new KeyNotFoundException($"No se encontró la marca con ID {id}."); }
            return Ok(new GenericResponse<BrandDetailDTO>("Marca obtenida exitosamente", result));
        }

        /// <summary>
        /// Crea una nueva marca en el sistema.
        /// </summary>
        [HttpPost("admin/brands")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateBrandAsync([FromBody] BrandCreateDTO brandCreate)
        {
            var result = await _brandService.CreateBrandAsync(brandCreate);
            string location = $"/api/admin/brands/{result}";
            return Created(location, new GenericResponse<string>("Marca creada exitosamente", result));
        }
        
        /// <summary>
        /// Actualiza una marca ya creada en el sistema mediante el id.
        /// </summary>
        [HttpPut("admin/brands/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBrandAsync(int id, [FromBody] BrandUpdateDTO brandUpdate)
        {
            var result = await _brandService.UpdateBrandAsync(id, brandUpdate);
            return Ok(new GenericResponse<BrandUpdateDTO>("Marca actualizada exitosamente", result));
        }
    }
}