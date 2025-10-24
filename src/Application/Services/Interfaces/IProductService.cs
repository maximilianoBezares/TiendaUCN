using TiendaUCN.src.Domain.Models;
using TiendaUCN.src.Application.DTO.ProductDTO;
using TiendaUCN.src.Application.DTO.ProductDTO.CustomerDTO;
using TiendaUCN.src.Application.DTO.ProductDTO.AdminDTO;

namespace TiendaUCN.src.Application.Services.Interfaces
{
    public interface IProductService
    {

        /// <summary>
        /// Retorna todos los productos para el administrador según los parámetros de búsqueda.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
        /// <returns>Una lista de productos filtrados para el administrador.</returns>
        Task<ListedProductsForAdminDTO> GetFilteredForAdminAsync(SearchParamsDTO searchParams);

        /// <summary>
        /// Retorna todos los productos para el cliente según los parámetros de búsqueda.
        /// </summary>
        /// <param name="searchParams">Parámetros de búsqueda para filtrar los productos.</param>
        /// <returns>Una lista de productos filtrados para el cliente.</returns>
        Task<ListedProductsForCustomerDTO> GetFilteredForCustomerAsync(SearchParamsDTO searchParams);

        /// <summary>
        /// Retorna un producto específico por su ID.
        /// </summary>
        /// <param name="id">El ID del producto a buscar.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el producto encontrado o null si no se encuentra.</returns>
        Task<ProductDetailDTO> GetByIdAsync(int id);

        /// <summary>
        /// Retorna un producto específico por su ID desde el punto de vista de un admin.
        /// </summary>
        /// <param name="id">El ID del producto a buscar.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el producto encontrado o null si no se encuentra.</returns>
        Task<ProductDetailAdminDTO> GetByIdForAdminAsync(int id);

        /// <summary>
        /// Crea un nuevo producto en el sistema.
        /// </summary>
        /// <param name="createProductDTO">Los datos del producto a crear.</param>
        /// <returns>Una tarea que representa la operación asíncrona, con el id del producto creado.</returns>
        Task<string> CreateAsync(CreateProductDTO createProductDTO);

        /// <summary>
        /// Añade una nueva imagen a un producto existente
        /// </summary>
        /// <param name="id"></param>id del producto para identificar el producto a añadir
        /// <param name="images"></param>lista de imagen de producto para incluir la nueva imagen
        /// <returns></returns>
        Task AddImagesAsync(int id, List<IFormFile> images);

        /// <summary>
        /// Cambia el estado activo de un producto por su ID.
        /// </summary>
        /// <param name="id">El ID del producto cuyo estado se cambiará.</param>
        Task ToggleActiveAsync(int id);

        /// <summary>
        /// Elimina de un producto por su ID.
        /// </summary>
        /// <param name="id">El ID del producto que se elimnara.</param>
        Task SoftDeleteAsync(int id);
        /// <summary>
        /// Elimina una imagen de un producto por su IdImagen.
        /// </summary>
        /// <param name="id">El ID de la imagen del producto que se elimnara.</param>
        Task DeleteImageAsync(int imageId);
        /// <summary>
        /// Cambia el descuento de un producto
        /// </summary>
        /// <param name="id"></param> Id del producto a cambiar
        /// <param name="discount"></param> descuento entre 0 a 100(%)
        /// <returns></returns>
        Task UpdateDiscountAsync(int id, int discount);
    }

}
