namespace SystemProcedure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateFileRepo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FileRepositoryItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsDeleted = c.Boolean(nullable: false),
                        byteContent = c.Binary(),
                        contentType = c.String(),
                        contentLenght = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LastDateUpdate = c.DateTime(),
                        LastUpdatedBy = c.String(),
                        CreatedDate = c.DateTime(),
                        CreatedBy = c.String(),
                        CompanyId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .Index(t => t.CompanyId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FileRepositoryItems", "CompanyId", "dbo.Companies");
            DropIndex("dbo.FileRepositoryItems", new[] { "CompanyId" });
            DropTable("dbo.FileRepositoryItems");
        }
    }
}
