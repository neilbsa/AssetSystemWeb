namespace SystemProcedure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveLifeSpanYear : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AssetHeaderDetails", "LifeSpanYear");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AssetHeaderDetails", "LifeSpanYear", c => c.Double(nullable: false));
        }
    }
}
