using Domapel.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Domapel.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasOne(c => c.Vendor)
                .WithMany(v => v.Customers)
                .HasForeignKey(c => c.VendorId);
            modelBuilder.Entity<Order>()
            .Property(o => o.CommissionValue)
            .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.Discount)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<Order>()
                .Property(o => o.FinalValue)
                .HasColumnType("nvarchar(max)");

            modelBuilder.Entity<Customer>()
                .Property(c => c.Complement)
                .IsRequired(false);
            modelBuilder.Entity<ComissionVendor>().HasNoKey(); 

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }

    }
}
