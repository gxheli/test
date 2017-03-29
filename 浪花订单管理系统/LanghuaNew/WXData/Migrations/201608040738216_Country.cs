namespace WXData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MenuItems",
                c => new
                    {
                        MenuItemID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        value = c.String(),
                        ItemType = c.Int(nullable: false),
                        WeiXinMenuID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MenuItemID)
                .ForeignKey("dbo.WeiXinMenus", t => t.WeiXinMenuID, cascadeDelete: true)
                .Index(t => t.WeiXinMenuID);
            
            CreateTable(
                "dbo.WeiXinMenus",
                c => new
                    {
                        WeiXinMenuID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.WeiXinMenuID);
            
            AlterColumn("dbo.LHNews", "update_time", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MenuItems", "WeiXinMenuID", "dbo.WeiXinMenus");
            DropIndex("dbo.MenuItems", new[] { "WeiXinMenuID" });
            AlterColumn("dbo.LHNews", "update_time", c => c.Long(nullable: false));
            DropTable("dbo.WeiXinMenus");
            DropTable("dbo.MenuItems");
        }
    }
}
