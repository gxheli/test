namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country21 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Suppliers", "EnableOnline", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Suppliers", "EnableOnline");
        }
    }
}
