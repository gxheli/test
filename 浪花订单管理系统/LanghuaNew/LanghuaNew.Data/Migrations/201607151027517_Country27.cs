namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country27 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SellControls",
                c => new
                    {
                        SellControlID = c.Int(nullable: false, identity: true),
                        SellControlName = c.String(),
                        SellControlNum = c.Int(nullable: false),
                        StartDate = c.DateTimeOffset(nullable: false, precision: 7),
                        MonthNum = c.Int(nullable: false),
                        SupplierID = c.Int(nullable: false),
                        FirstServiceItem_ServiceItemID = c.Int(),
                        SecondServiceItem_ServiceItemID = c.Int(),
                    })
                .PrimaryKey(t => t.SellControlID)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID, cascadeDelete: true)
                .ForeignKey("dbo.ServiceItems", t => t.FirstServiceItem_ServiceItemID)
                .ForeignKey("dbo.ServiceItems", t => t.SecondServiceItem_ServiceItemID)
                .Index(t => t.SupplierID)
                .Index(t => t.FirstServiceItem_ServiceItemID)
                .Index(t => t.SecondServiceItem_ServiceItemID);
            
            CreateTable(
                "dbo.SystemLogs",
                c => new
                    {
                        SystemLogID = c.Int(nullable: false, identity: true),
                        Operate = c.String(),
                        OperateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        UserID = c.Int(nullable: false),
                        UserName = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.SystemLogID);
            
            AddColumn("dbo.TBOrderStates", "SendUserID", c => c.Int(nullable: false));
            AddColumn("dbo.TBOrderStates", "SendUserName", c => c.String());
            AddColumn("dbo.TBOrderStates", "SendTime", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SellControls", "SecondServiceItem_ServiceItemID", "dbo.ServiceItems");
            DropForeignKey("dbo.SellControls", "FirstServiceItem_ServiceItemID", "dbo.ServiceItems");
            DropForeignKey("dbo.SellControls", "SupplierID", "dbo.Suppliers");
            DropIndex("dbo.SellControls", new[] { "SecondServiceItem_ServiceItemID" });
            DropIndex("dbo.SellControls", new[] { "FirstServiceItem_ServiceItemID" });
            DropIndex("dbo.SellControls", new[] { "SupplierID" });
            DropColumn("dbo.TBOrderStates", "SendTime");
            DropColumn("dbo.TBOrderStates", "SendUserName");
            DropColumn("dbo.TBOrderStates", "SendUserID");
            DropTable("dbo.SystemLogs");
            DropTable("dbo.SellControls");
        }
    }
}
