namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceItems", "ElementContent", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceItems", "ElementContent");
        }
    }
}
