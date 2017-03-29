namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExtraServiceHistories", "ExtraServiceID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExtraServiceHistories", "ExtraServiceID");
        }
    }
}
