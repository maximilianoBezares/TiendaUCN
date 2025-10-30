using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Application.DTO.UserCrudAdminDTOs
{
    public class AdminUserQueryParameters
    {
        private const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? Role { get; set; }
        public string? Status { get; set; } 
        public string? Email { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }

        public string SortBy { get; set; } = "RegisteredAt";
        public string SortDirection { get; set; } = "desc";
    }
}
