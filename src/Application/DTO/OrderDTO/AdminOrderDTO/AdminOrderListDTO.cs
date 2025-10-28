namespace TiendaUCN.src.Application.DTO.OrderDTO.AdminDTO
{
    public class AdminOrderListDTO
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string Status { get; set; }
        public required string CustomerEmail { get; set; }
        public required string CustomerName { get; set; }
        public required string Total { get; set; }
        public int ItemsCount { get; set; }
    }
}
