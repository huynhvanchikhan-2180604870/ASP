
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SchoolSuppliesStore.Models;
namespace SchoolSuppliesStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
