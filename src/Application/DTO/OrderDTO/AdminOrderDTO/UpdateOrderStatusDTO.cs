using System.ComponentModel.DataAnnotations;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.Application.DTO.OrderDTO.AdminDTO
{
    public class UpdateOrderStatusDTO
    {
        [Required(ErrorMessage = "El estado es obligatorio.")]
        [EnumDataType(
            typeof(OrderStatus),
            ErrorMessage = "El estado proporcionado no es válido. Debe ser uno de estos: Pending, Paid, Shipped, Delivered o Cancelled"
        )]
        public required string Status { get; set; }

        public string? Note { get; set; }
    }
}
