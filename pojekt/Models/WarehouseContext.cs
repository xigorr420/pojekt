using Microsoft.EntityFrameworkCore;

namespace pojekt.Models
{
    public class WarehouseContext : DbContext
    {
        public WarehouseContext(DbContextOptions<WarehouseContext> options)
            : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<UserDetailsModel> UserDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relacja 1-1: User -> UserDetails
            modelBuilder.Entity<UserModel>()
                .HasOne(u => u.UserDetails)
                .WithOne(d => d.User)
                .HasForeignKey<UserDetailsModel>(d => d.UserId);

            // Relacja 1-wiele: User -> Orders
            modelBuilder.Entity<OrderModel>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            // Relacja 1-wiele: Product -> Orders
            modelBuilder.Entity<OrderModel>()
                .HasOne(o => o.Product)
                .WithMany(p => p.Orders)
                .HasForeignKey(o => o.ProductId);
        }
    }
}
