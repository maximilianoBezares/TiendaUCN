namespace TiendaUCN.src.Application.DTO.OrderDTO.AdminDTO
{
    public class ListedAdminOrdersDTO
    {
        public List<AdminOrderListDTO> Orders { get; set; } = new List<AdminOrderListDTO>();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
