namespace SystemProcedure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDeptCompany : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Branches", "CompanyId", c => c.Int());
            CreateIndex("dbo.Branches", "CompanyId");
            AddForeignKey("dbo.Branches", "CompanyId", "dbo.Companies", "CompanyId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Branches", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Branches", new[] { "CompanyId" });
            DropColumn("dbo.Branches", "CompanyId");
        }
    }
}
