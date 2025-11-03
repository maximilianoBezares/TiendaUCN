using Bogus.DataSets;

namespace TiendaUCN.src.Application.DTO.ProductDTO.AdminDTO
{
    public class ProductForAdminDTO
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public List<string> ImagesURL { get; set; } = new List<string>();
        public required string Price { get; set; }
        public required int Discount { get; set; }
        public required string FinalPrice { get; set; }
        public required int Stock { get; set; }
        public required string StockIndicator { get; set; }
        public required string CategoryName { get; set; }
        public required string BrandName { get; set; }
        public required string StatusName { get; set; }
        public required bool IsAvailable { get; set; }        
        public required bool Deleted { get; set; }
        
    }
}
