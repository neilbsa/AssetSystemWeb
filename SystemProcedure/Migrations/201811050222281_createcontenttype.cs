namespace SystemProcedure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createcontenttype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileRepositoryItems", "DocumentType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FileRepositoryItems", "DocumentType");
        }
    }
}
