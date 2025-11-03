namespace TiendaUCN.src.Application.DTO.OrderDTO.AdminDTO
{
    // Reutiliza el OrderItemDTO que ya tienes
    public class AdminOrderDetailDTO
    {
        public int Id { get; set; }
        public required string Code { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required string Total { get; set; }
        public required string SubTotal { get; set; }
        public required string Savings { get; set; }
        public required CustomerInfoDTO Customer { get; set; }
        public required List<OrderItemDTO> Items { get; set; }
    }

    public class CustomerInfoDTO
    {
        public int UserId { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
    }
}
