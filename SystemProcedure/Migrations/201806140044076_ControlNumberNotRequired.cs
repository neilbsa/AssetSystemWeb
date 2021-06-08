namespace SystemProcedure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ControlNumberNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AssetItemDetails", "ControlNumber", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AssetItemDetails", "ControlNumber", c => c.String(nullable: false));
        }
    }
}
