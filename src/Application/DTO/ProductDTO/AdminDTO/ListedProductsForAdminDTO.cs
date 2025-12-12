using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.Application.DTO.ProductDTO.AdminDTO
{
    public class ListedProductsForAdminDTO
    {
        public List<ProductDetailAdminDTO> Products { get; set; } = new List<ProductDetailAdminDTO>();

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }
    }
}
