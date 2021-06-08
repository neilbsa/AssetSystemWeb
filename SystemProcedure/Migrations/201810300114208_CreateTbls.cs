namespace SystemProcedure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTbls : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileRepositoryItems", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.FileRepositoryItems", "AssetHeaderDetails_Id", c => c.Int());
            CreateIndex("dbo.FileRepositoryItems", "AssetHeaderDetails_Id");
            AddForeignKey("dbo.FileRepositoryItems", "AssetHeaderDetails_Id", "dbo.AssetHeaderDetails", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileRepositoryItems", "AssetHeaderDetails_Id", "dbo.AssetHeaderDetails");
            DropIndex("dbo.FileRepositoryItems", new[] { "AssetHeaderDetails_Id" });
            DropColumn("dbo.FileRepositoryItems", "AssetHeaderDetails_Id");
            DropColumn("dbo.FileRepositoryItems", "Discriminator");
        }
    }
}
