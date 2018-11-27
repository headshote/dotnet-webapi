namespace WebApplicationExercise.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CurrencyFormat : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "PriceUSD", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Products", "Price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Products", "Price", c => c.Double(nullable: false));
            DropColumn("dbo.Products", "PriceUSD");
        }
    }
}
