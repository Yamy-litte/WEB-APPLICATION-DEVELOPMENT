namespace RestaurantWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 500),
                        ImageUrl = c.String(maxLength: 250),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Dishes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                        Description = c.String(maxLength: 1000),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PreparationTime = c.Int(nullable: false),
                        Status = c.String(nullable: false, maxLength: 30),
                        CategoryId = c.Int(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.ComboItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ComboId = c.Int(nullable: false),
                        DishId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Comboes", t => t.ComboId)
                .ForeignKey("dbo.Dishes", t => t.DishId)
                .Index(t => t.ComboId)
                .Index(t => t.DishId);
            
            CreateTable(
                "dbo.Comboes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                        Description = c.String(maxLength: 500),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ImageUrl = c.String(maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        DishId = c.Int(),
                        ComboId = c.Int(),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Note = c.String(maxLength: 500),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Comboes", t => t.ComboId)
                .ForeignKey("dbo.Dishes", t => t.DishId)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .Index(t => t.OrderId)
                .Index(t => t.DishId)
                .Index(t => t.ComboId);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ReservationId = c.Int(),
                        TableId = c.Int(),
                        OrderType = c.String(nullable: false, maxLength: 30),
                        Status = c.String(nullable: false, maxLength: 30),
                        DeliveryAddress = c.String(maxLength: 500),
                        Phone = c.String(maxLength: 20),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DiscountAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DeliveryFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Note = c.String(maxLength: 500),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Reservations", t => t.ReservationId)
                .ForeignKey("dbo.RestaurantTables", t => t.TableId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ReservationId)
                .Index(t => t.TableId);
            
            CreateTable(
                "dbo.OrderPromotions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        PromotionId = c.Int(nullable: false),
                        DiscountAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .ForeignKey("dbo.Promotions", t => t.PromotionId)
                .Index(t => t.OrderId)
                .Index(t => t.PromotionId);
            
            CreateTable(
                "dbo.Promotions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 50),
                        Name = c.String(nullable: false, maxLength: 150),
                        Description = c.String(maxLength: 500),
                        DiscountType = c.String(nullable: false, maxLength: 30),
                        DiscountValue = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MinimumOrderAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaximumDiscount = c.Decimal(precision: 18, scale: 2),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        UsageLimit = c.Int(),
                        UsedCount = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        PaymentMethod = c.String(nullable: false, maxLength: 30),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.String(nullable: false, maxLength: 30),
                        PaymentDate = c.DateTime(nullable: false),
                        TransactionCode = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TableId = c.Int(nullable: false),
                        ReservationDate = c.DateTime(nullable: false),
                        PartySize = c.Int(nullable: false),
                        SpecialRequest = c.String(maxLength: 500),
                        Status = c.String(nullable: false, maxLength: 30),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RestaurantTables", t => t.TableId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.TableId);
            
            CreateTable(
                "dbo.RestaurantTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TableNumber = c.String(nullable: false, maxLength: 20),
                        Capacity = c.Int(nullable: false),
                        Zone = c.String(nullable: false, maxLength: 50),
                        Status = c.String(nullable: false, maxLength: 30),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(nullable: false, maxLength: 100),
                        Email = c.String(nullable: false, maxLength: 150),
                        PasswordHash = c.String(nullable: false),
                        Phone = c.String(maxLength: 20),
                        Address = c.String(maxLength: 250),
                        Role = c.String(nullable: false, maxLength: 30),
                        IsActive = c.Boolean(nullable: false),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Reviews",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        DishId = c.Int(),
                        OrderId = c.Int(nullable: false),
                        Rating = c.Int(nullable: false),
                        Comment = c.String(maxLength: 1000),
                        CreatedAt = c.DateTime(nullable: false),
                        IsApproved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dishes", t => t.DishId)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.DishId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.DishImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DishId = c.Int(nullable: false),
                        ImageUrl = c.String(nullable: false, maxLength: 500),
                        IsPrimary = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Dishes", t => t.DishId)
                .Index(t => t.DishId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DishImages", "DishId", "dbo.Dishes");
            DropForeignKey("dbo.ComboItems", "DishId", "dbo.Dishes");
            DropForeignKey("dbo.ComboItems", "ComboId", "dbo.Comboes");
            DropForeignKey("dbo.OrderItems", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "TableId", "dbo.RestaurantTables");
            DropForeignKey("dbo.Orders", "ReservationId", "dbo.Reservations");
            DropForeignKey("dbo.Reservations", "UserId", "dbo.Users");
            DropForeignKey("dbo.Reviews", "UserId", "dbo.Users");
            DropForeignKey("dbo.Reviews", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Reviews", "DishId", "dbo.Dishes");
            DropForeignKey("dbo.Reservations", "TableId", "dbo.RestaurantTables");
            DropForeignKey("dbo.Payments", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderPromotions", "PromotionId", "dbo.Promotions");
            DropForeignKey("dbo.OrderPromotions", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderItems", "DishId", "dbo.Dishes");
            DropForeignKey("dbo.OrderItems", "ComboId", "dbo.Comboes");
            DropForeignKey("dbo.Dishes", "CategoryId", "dbo.Categories");
            DropIndex("dbo.DishImages", new[] { "DishId" });
            DropIndex("dbo.Reviews", new[] { "OrderId" });
            DropIndex("dbo.Reviews", new[] { "DishId" });
            DropIndex("dbo.Reviews", new[] { "UserId" });
            DropIndex("dbo.Reservations", new[] { "TableId" });
            DropIndex("dbo.Reservations", new[] { "UserId" });
            DropIndex("dbo.Payments", new[] { "OrderId" });
            DropIndex("dbo.OrderPromotions", new[] { "PromotionId" });
            DropIndex("dbo.OrderPromotions", new[] { "OrderId" });
            DropIndex("dbo.Orders", new[] { "TableId" });
            DropIndex("dbo.Orders", new[] { "ReservationId" });
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropIndex("dbo.OrderItems", new[] { "ComboId" });
            DropIndex("dbo.OrderItems", new[] { "DishId" });
            DropIndex("dbo.OrderItems", new[] { "OrderId" });
            DropIndex("dbo.ComboItems", new[] { "DishId" });
            DropIndex("dbo.ComboItems", new[] { "ComboId" });
            DropIndex("dbo.Dishes", new[] { "CategoryId" });
            DropTable("dbo.DishImages");
            DropTable("dbo.Reviews");
            DropTable("dbo.Users");
            DropTable("dbo.RestaurantTables");
            DropTable("dbo.Reservations");
            DropTable("dbo.Payments");
            DropTable("dbo.Promotions");
            DropTable("dbo.OrderPromotions");
            DropTable("dbo.Orders");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Comboes");
            DropTable("dbo.ComboItems");
            DropTable("dbo.Dishes");
            DropTable("dbo.Categories");
        }
    }
}
