namespace SystemProcedure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFileinConsignment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Consignments", "fileId", c => c.Int());
            CreateIndex("dbo.Consignments", "fileId");
            AddForeignKey("dbo.Consignments", "fileId", "dbo.FileRepositoryItems", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Consignments", "fileId", "dbo.FileRepositoryItems");
            DropIndex("dbo.Consignments", new[] { "fileId" });
            DropColumn("dbo.Consignments", "fileId");
        }
    }
}
