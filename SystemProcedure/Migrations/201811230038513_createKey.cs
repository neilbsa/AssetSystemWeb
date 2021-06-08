namespace SystemProcedure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FileRepositoryItems", "AssetHeaderDetails_Id", "dbo.AssetHeaderDetails");
            DropIndex("dbo.FileRepositoryItems", new[] { "AssetHeaderDetails_Id" });
            RenameColumn(table: "dbo.FileRepositoryItems", name: "AssetHeaderDetails_Id", newName: "KeyId");
            AlterColumn("dbo.FileRepositoryItems", "KeyId", c => c.Int(nullable: false));
            CreateIndex("dbo.FileRepositoryItems", "KeyId");
            AddForeignKey("dbo.FileRepositoryItems", "KeyId", "dbo.AssetHeaderDetails", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileRepositoryItems", "KeyId", "dbo.AssetHeaderDetails");
            DropIndex("dbo.FileRepositoryItems", new[] { "KeyId" });
            AlterColumn("dbo.FileRepositoryItems", "KeyId", c => c.Int());
            RenameColumn(table: "dbo.FileRepositoryItems", name: "KeyId", newName: "AssetHeaderDetails_Id");
            CreateIndex("dbo.FileRepositoryItems", "AssetHeaderDetails_Id");
            AddForeignKey("dbo.FileRepositoryItems", "AssetHeaderDetails_Id", "dbo.AssetHeaderDetails", "Id");
        }
    }
}
