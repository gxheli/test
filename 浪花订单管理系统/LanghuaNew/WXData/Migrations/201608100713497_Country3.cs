namespace WXData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MenuItems", "Text", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MenuItems", "Text");
        }
    }
}
