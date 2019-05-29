using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EcommerceProject.Models;

namespace EcommerceProject.Models
{
    public class EcommerceProjectContext : DbContext
    {
        public EcommerceProjectContext (DbContextOptions<EcommerceProjectContext> options)
            : base(options)
        {
        }
        public DbSet<EcommerceProject.Models.Product> Product { get; set; }
        public DbSet<EcommerceProject.Models.Order> Order { get; set; }
        public DbSet<EcommerceProject.Models.OrderItem> OrderItem { get; set; }

      
    }
}
