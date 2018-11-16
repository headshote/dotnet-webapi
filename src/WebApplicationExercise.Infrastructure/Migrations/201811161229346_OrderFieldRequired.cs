namespace WebApplicationExercise.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OrderFieldRequired : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Products", new[] { "Order_Id" });
            AlterColumn("dbo.Products", "Order_Id", c => c.Guid(nullable: false));
            CreateIndex("dbo.Products", "Order_Id");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Products", new[] { "Order_Id" });
            AlterColumn("dbo.Products", "Order_Id", c => c.Guid());
            CreateIndex("dbo.Products", "Order_Id");
        }
    }
}
