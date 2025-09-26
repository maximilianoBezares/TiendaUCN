using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TiendaUCN.src.Domain.Models;

namespace TiendaUCN.src.Infrastructure.Data
{
    public class DataContext : IdentityDbContext<User, Role, int>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options) { }

        public DbSet<VerificationCode> VerificationCodes { get; set; } = null!;
        public DbSet<Image> Images { get; set; } = null!;
    }
}
