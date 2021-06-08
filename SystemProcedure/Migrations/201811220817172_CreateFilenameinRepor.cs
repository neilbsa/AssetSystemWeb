namespace SystemProcedure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateFilenameinRepor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FileRepositoryItems", "FileName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FileRepositoryItems", "FileName");
        }
    }
}
