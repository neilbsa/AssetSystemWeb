namespace SystemProcedure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateStatusConsignmentAndFile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consignments", "Status", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Consignments", "Status");
        }
    }
}
