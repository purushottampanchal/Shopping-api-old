using Microsoft.EntityFrameworkCore;
using shoppingApi.Models.AuthenticationModels;
using ShoppingApi.Models;

namespace ShoppingApi.MyDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Catlog> Catlogs { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<CartItem> CartItems { get; set; }
    
        public virtual DbSet<CardDetail> CardDetails { get; set; }
        public virtual DbSet<Address> Addresses{ get; set; }


    }
}
