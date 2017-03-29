namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country38 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ServiceRuleLogs", "ServiceRuleID", "dbo.ServiceRules");
            DropForeignKey("dbo.ServiceRuleServiceItems", "ServiceRule_ServiceRuleID", "dbo.ServiceRules");
            DropForeignKey("dbo.ServiceRuleServiceItems", "ServiceItem_ServiceItemID", "dbo.ServiceItems");
            DropIndex("dbo.ServiceRuleLogs", new[] { "ServiceRuleID" });
            DropIndex("dbo.ServiceRuleServiceItems", new[] { "ServiceRule_ServiceRuleID" });
            DropIndex("dbo.ServiceRuleServiceItems", new[] { "ServiceItem_ServiceItemID" });
            DropTable("dbo.ServiceRules");
            DropTable("dbo.ServiceRuleLogs");
            DropTable("dbo.ServiceRuleServiceItems");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ServiceRuleServiceItems",
                c => new
                    {
                        ServiceRule_ServiceRuleID = c.Int(nullable: false),
                        ServiceItem_ServiceItemID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ServiceRule_ServiceRuleID, t.ServiceItem_ServiceItemID });
            
            CreateTable(
                "dbo.ServiceRuleLogs",
                c => new
                    {
                        ServiceRuleLogID = c.Int(nullable: false, identity: true),
                        Operate = c.String(),
                        OperateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        UserID = c.Int(nullable: false),
                        UserName = c.String(),
                        Remark = c.String(),
                        ServiceRuleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ServiceRuleLogID);
            
            CreateTable(
                "dbo.ServiceRules",
                c => new
                    {
                        ServiceRuleID = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTimeOffset(nullable: false, precision: 7),
                        EndTime = c.DateTimeOffset(nullable: false, precision: 7),
                        UseState = c.Int(nullable: false),
                        SelectRuleType = c.Int(nullable: false),
                        RangeStart = c.DateTimeOffset(nullable: false, precision: 7),
                        RangeEnd = c.DateTimeOffset(nullable: false, precision: 7),
                        Week = c.String(),
                        IsDouble = c.Boolean(nullable: false),
                        UseDate = c.String(),
                    })
                .PrimaryKey(t => t.ServiceRuleID);
            
            CreateIndex("dbo.ServiceRuleServiceItems", "ServiceItem_ServiceItemID");
            CreateIndex("dbo.ServiceRuleServiceItems", "ServiceRule_ServiceRuleID");
            CreateIndex("dbo.ServiceRuleLogs", "ServiceRuleID");
            AddForeignKey("dbo.ServiceRuleServiceItems", "ServiceItem_ServiceItemID", "dbo.ServiceItems", "ServiceItemID", cascadeDelete: true);
            AddForeignKey("dbo.ServiceRuleServiceItems", "ServiceRule_ServiceRuleID", "dbo.ServiceRules", "ServiceRuleID", cascadeDelete: true);
            AddForeignKey("dbo.ServiceRuleLogs", "ServiceRuleID", "dbo.ServiceRules", "ServiceRuleID", cascadeDelete: true);
        }
    }
}
