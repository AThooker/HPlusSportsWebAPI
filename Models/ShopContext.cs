using Microsoft.EntityFrameworkCore;

namespace HPlusSport.Models
{
    public class ShopContext : DbContext
    {
        //Constructor
        public ShopContext(DbContextOptions options):base(options){}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Category has list of products which has one category with categoryId FK
            modelBuilder.Entity<Category>().HasMany(c => c.Products).WithOne(a => a.Category).HasForeignKey(a => a.CategoryId);
            //Oder has list of products
            modelBuilder.Entity<Order>().HasMany(o => o.Products);
            //Order has a user
            modelBuilder.Entity<Order>().HasOne(o => o.User);
            //User has many orders with one user with userId FK
            modelBuilder.Entity<User>().HasMany(u => u.Orders).WithOne(o => o.User).HasForeignKey(o => o.UserId);

            modelBuilder.Seed();
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<User> Users { get; set; }
    }
}