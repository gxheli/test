namespace WXData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MenuItems", "RowNo", c => c.Int(nullable: false));
            AddColumn("dbo.WeiXinMenus", "RowNo", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WeiXinMenus", "RowNo");
            DropColumn("dbo.MenuItems", "RowNo");
        }
    }
}
