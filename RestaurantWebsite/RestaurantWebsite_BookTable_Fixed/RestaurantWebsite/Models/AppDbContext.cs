using System.Data.Entity;

namespace RestaurantWebsite.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
            : base("name=RestaurantWebsite")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<DishImage> DishImages { get; set; }
        public DbSet<RestaurantTable> RestaurantTables { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<OrderPromotion> OrderPromotions { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboItem> ComboItems { get; set; }
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User - Order
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .WillCascadeOnDelete(false);

            // User - Reservation
            modelBuilder.Entity<Reservation>()
                .HasRequired(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .WillCascadeOnDelete(false);

            // User - Review
            modelBuilder.Entity<Review>()
                .HasRequired(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .WillCascadeOnDelete(false);

            // Order - Reservation
            modelBuilder.Entity<Order>()
                .HasOptional(o => o.Reservation)
                .WithMany(r => r.Orders)
                .HasForeignKey(o => o.ReservationId)
                .WillCascadeOnDelete(false);

            // Order - RestaurantTable
            modelBuilder.Entity<Order>()
                .HasOptional(o => o.Table)
                .WithMany(t => t.Orders)
                .HasForeignKey(o => o.TableId)
                .WillCascadeOnDelete(false);

            // Reservation - RestaurantTable
            modelBuilder.Entity<Reservation>()
                .HasRequired(r => r.Table)
                .WithMany(t => t.Reservations)
                .HasForeignKey(r => r.TableId)
                .WillCascadeOnDelete(false);

            // Category - Dish
            modelBuilder.Entity<Dish>()
                .HasRequired(d => d.Category)
                .WithMany(c => c.Dishes)
                .HasForeignKey(d => d.CategoryId)
                .WillCascadeOnDelete(false);

            // Dish - DishImage
            modelBuilder.Entity<DishImage>()
                .HasRequired(di => di.Dish)
                .WithMany(d => d.DishImages)
                .HasForeignKey(di => di.DishId)
                .WillCascadeOnDelete(false);

            // Order - OrderItem
            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .WillCascadeOnDelete(false);

            // OrderItem - Dish
            modelBuilder.Entity<OrderItem>()
                .HasOptional(oi => oi.Dish)
                .WithMany(d => d.OrderItems)
                .HasForeignKey(oi => oi.DishId)
                .WillCascadeOnDelete(false);

            // OrderItem - Combo
            modelBuilder.Entity<OrderItem>()
                .HasOptional(oi => oi.Combo)
                .WithMany(c => c.OrderItems)
                .HasForeignKey(oi => oi.ComboId)
                .WillCascadeOnDelete(false);

            // Combo - ComboItem
            modelBuilder.Entity<ComboItem>()
                .HasRequired(ci => ci.Combo)
                .WithMany(c => c.ComboItems)
                .HasForeignKey(ci => ci.ComboId)
                .WillCascadeOnDelete(false);

            // ComboItem - Dish
            modelBuilder.Entity<ComboItem>()
                .HasRequired(ci => ci.Dish)
                .WithMany(d => d.ComboItems)
                .HasForeignKey(ci => ci.DishId)
                .WillCascadeOnDelete(false);

            // Order - Payment
            modelBuilder.Entity<Payment>()
                .HasRequired(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .WillCascadeOnDelete(false);

            // Order - OrderPromotion
            modelBuilder.Entity<OrderPromotion>()
                .HasRequired(op => op.Order)
                .WithMany(o => o.OrderPromotions)
                .HasForeignKey(op => op.OrderId)
                .WillCascadeOnDelete(false);

            // Promotion - OrderPromotion
            modelBuilder.Entity<OrderPromotion>()
                .HasRequired(op => op.Promotion)
                .WithMany(p => p.OrderPromotions)
                .HasForeignKey(op => op.PromotionId)
                .WillCascadeOnDelete(false);

            // Order - Review
            modelBuilder.Entity<Review>()
                .HasOptional(r => r.Order)
                .WithMany(o => o.Reviews)
                .HasForeignKey(r => r.OrderId)
                .WillCascadeOnDelete(false);

            // Dish - Review
            modelBuilder.Entity<Review>()
                .HasOptional(r => r.Dish)
                .WithMany(d => d.Reviews)
                .HasForeignKey(r => r.DishId)
                .WillCascadeOnDelete(false);
        }
    }
}