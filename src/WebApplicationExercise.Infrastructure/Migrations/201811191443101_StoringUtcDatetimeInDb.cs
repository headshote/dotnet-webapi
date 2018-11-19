namespace WebApplicationExercise.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StoringUtcDatetimeInDb : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Orders", "CreatedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Orders", "CreatedDate", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
    }
}
