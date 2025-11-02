using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TiendaUCN.src.Application.DTO.UserCrudAdminDTOs
{
    public class UserListDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Rut { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string Status { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}