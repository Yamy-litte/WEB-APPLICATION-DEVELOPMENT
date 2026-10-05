namespace RestaurantWebsite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeReviewOrderOptional : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Reviews", new[] { "OrderId" });
            AlterColumn("dbo.Reviews", "OrderId", c => c.Int());
            CreateIndex("dbo.Reviews", "OrderId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Reviews", new[] { "OrderId" });
            AlterColumn("dbo.Reviews", "OrderId", c => c.Int(nullable: false));
            CreateIndex("dbo.Reviews", "OrderId");
        }
    }
}
