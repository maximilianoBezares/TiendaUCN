using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TiendaUCN.src.api.Controllers;
using TiendaUCN.src.Application.DTO;
using TiendaUCN.src.Application.DTO.ProductDTO;
using TiendaUCN.src.Application.DTO.ProductDTO.AdminDTO;
using TiendaUCN.src.Application.DTO.ProductDTO.CustomerDTO;
using TiendaUCN.src.Application.Services.Interfaces;

namespace TiendaUCN.src.API.Controllers
{
    /// <summary>
    /// Controlador para manejar las operaciones relacionadas con los productos.
    /// </summary>
    public class ProductController : BaseController
    {
        /// <summary>
        /// Controlador para manejar las operaciones relacionadas con los productos.
        /// </summary>
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Obtiene todos los productos para el administrador con los parámetros de búsqueda especificados.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
        /// <returns>Una lista de productos filtrados para el administrador.</returns>
        [HttpGet("admin/products")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllForAdminAsync(
            [FromQuery] SearchParamsDTO searchParams
        )
        {
            var result = await _productService.GetFilteredForAdminAsync(searchParams);
            if (result == null || result.Products.Count == 0)
            {
                throw new KeyNotFoundException("No se encontraron productos.");
            }
            return Ok(
                new GenericResponse<ListedProductsForAdminDTO>(
                    "Productos obtenidos exitosamente",
                    result
                )
            );
        }

        [HttpGet("customer/products")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllForCustomerAsync(
            [FromQuery] SearchParamsDTO searchParams
        )
        {
            var result = await _productService.GetFilteredForCustomerAsync(searchParams);
            if (result == null || result.Products.Count == 0)
            {
                throw new KeyNotFoundException("No se encontraron productos.");
            }
            return Ok(
                new GenericResponse<ListedProductsForCustomerDTO>(
                    "Productos obtenidos exitosamente",
                    result
                )
            );
        }

        /// <summary>
        /// Obtiene un producto específico para el cliente.
        /// </summary>
        /// <param name="id">ID del producto a obtener.</param>
        /// <returns>El producto solicitado.</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIdForCustomerAsync(int id)
        {
            var result = await _productService.GetByIdAsync(id);
            if (result == null)
            {
                throw new KeyNotFoundException("Producto no encontrado.");
            }
            return Ok(
                new GenericResponse<ProductDetailDTO>("Producto obtenido exitosamente", result)
            );
        }

        /// <summary>
        /// Obtiene un producto específico para el admin.
        /// </summary>
        /// <param name="id">ID del producto a obtener.</param>
        /// <returns>El producto solicitado.</returns>
        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByIdForAdminAsync(int id)
        {
            var result = await _productService.GetByIdForAdminAsync(id);
            if (result == null)
            {
                throw new KeyNotFoundException("Producto no encontrado.");
            }
            return Ok(
                new GenericResponse<ProductDetailAdminDTO>("Producto obtenido exitosamente", result)
            );
        }

        /// <summary>
        /// Crea un nuevo producto en el sistema.
        /// </summary>
        /// <param name="createProductDTO">Los datos del producto a crear.</param>
        /// <returns>El ID del producto creado.</returns>
        [HttpPost()]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAsync([FromForm] CreateProductDTO createProductDTO)
        {
            var result = await _productService.CreateAsync(createProductDTO);
            return Created(
                $"/api/product/{result}",
                new GenericResponse<string>("Producto creado exitosamente", result)
            );
        }

        /// <summary>
        /// Cambia el estado activo de un producto por su ID.
        /// </summary>
        /// <param name="id">El ID del producto cuyo estado se cambiará.</param>
        /// <returns>Una respuesta que indica el resultado de la operación.</returns>
        [HttpPatch("{id}/toggle-active")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleActiveAsync(int id)
        {
            await _productService.ToggleActiveAsync(id);
            return Ok(
                new GenericResponse<string>(
                    "Estado del producto actualizado exitosamente",
                    "El estado del producto ha sido cambiado."
                )
            );
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            await _productService.SoftDeleteAsync(id);

            // Retornamos 200 OK con mensaje, siendo coherentes con tu ToggleActiveAsync
            return Ok(
                new GenericResponse<string>(
                    "Producto eliminado exitosamente",
                    "El producto ha sido marcado como eliminado."
                )
            );
        }
    }
}
