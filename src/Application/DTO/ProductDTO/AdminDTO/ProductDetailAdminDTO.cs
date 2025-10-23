namespace TiendaUCN.src.Application.DTO.ProductDTO.AdminDTO
{
    public class ProductDetailAdminDTO
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public string? MainImageURL { get; set; }
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
        public required DateTime UpdatedAt { get; set; }
    }
}
