namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkTableDisplayItems",
                c => new
                    {
                        WorkTableDisplayItemID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        MyNotfilledCount = c.Boolean(nullable: false),
                        MyFilledCount = c.Boolean(nullable: false),
                        MyNoPayCount = c.Boolean(nullable: false),
                        MySencondFullCount = c.Boolean(nullable: false),
                        MyTodayOrderCount = c.Boolean(nullable: false),
                        MyTodaySales = c.Boolean(nullable: false),
                        MyTodayProfits = c.Boolean(nullable: false),
                        TodayOrderCount = c.Boolean(nullable: false),
                        OnCheckCount = c.Boolean(nullable: false),
                        CheckCount = c.Boolean(nullable: false),
                        NeedServiceCount = c.Boolean(nullable: false),
                        TodayTravelNum = c.Boolean(nullable: false),
                        TodayProfits = c.Boolean(nullable: false),
                        TodaySales = c.Boolean(nullable: false),
                        ThisMonthTravelNum = c.Boolean(nullable: false),
                        WeixinBindCount = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.WorkTableDisplayItemID)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID);
            
            CreateIndex("dbo.ServiceItemHistories", "SupplierID");
            AddForeignKey("dbo.ServiceItemHistories", "SupplierID", "dbo.Suppliers", "SupplierID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkTableDisplayItems", "UserID", "dbo.Users");
            DropForeignKey("dbo.ServiceItemHistories", "SupplierID", "dbo.Suppliers");
            DropIndex("dbo.WorkTableDisplayItems", new[] { "UserID" });
            DropIndex("dbo.ServiceItemHistories", new[] { "SupplierID" });
            DropTable("dbo.WorkTableDisplayItems");
        }
    }
}
