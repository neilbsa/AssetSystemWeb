namespace SystemProcedure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Test : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AssetHeaderDetails", "WarrantyMonths", c => c.Int(nullable: false));
            AddColumn("dbo.AssetHeaderDetails", "LifeSpanMonths", c => c.Double(nullable: false));
            AddColumn("dbo.AssetItemDetails", "ControlNumber", c => c.String(nullable: false));
            DropColumn("dbo.AssetHeaderDetails", "WarrantyYears");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AssetHeaderDetails", "WarrantyYears", c => c.Int(nullable: false));
            DropColumn("dbo.AssetItemDetails", "ControlNumber");
            DropColumn("dbo.AssetHeaderDetails", "LifeSpanMonths");
            DropColumn("dbo.AssetHeaderDetails", "WarrantyMonths");
        }
    }
}
