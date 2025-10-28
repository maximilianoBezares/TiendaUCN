using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TiendaUCN.src.Application.DTO.ProductDTO;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.Application.DTO.OrderDTO.AdminDTO
{
    public class AdminOrderSearchParamsDTO : SearchParamsDTO
    {
        public OrderStatus? Status { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? CustomerEmail { get; set; }
        public string? OrderCode { get; set; }
        public string? SortBy { get; set; }
    }
}
