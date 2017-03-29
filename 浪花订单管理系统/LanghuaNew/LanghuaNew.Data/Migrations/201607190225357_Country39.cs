namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country39 : DbMigration
    {
        public override void Up()
        {
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
                .PrimaryKey(t => t.ServiceRuleLogID)
                .ForeignKey("dbo.ServiceRules", t => t.ServiceRuleID, cascadeDelete: true)
                .Index(t => t.ServiceRuleID);
            
            CreateTable(
                "dbo.ServiceRuleServiceItems",
                c => new
                    {
                        ServiceRule_ServiceRuleID = c.Int(nullable: false),
                        ServiceItem_ServiceItemID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ServiceRule_ServiceRuleID, t.ServiceItem_ServiceItemID })
                .ForeignKey("dbo.ServiceRules", t => t.ServiceRule_ServiceRuleID, cascadeDelete: true)
                .ForeignKey("dbo.ServiceItems", t => t.ServiceItem_ServiceItemID, cascadeDelete: true)
                .Index(t => t.ServiceRule_ServiceRuleID)
                .Index(t => t.ServiceItem_ServiceItemID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceRuleServiceItems", "ServiceItem_ServiceItemID", "dbo.ServiceItems");
            DropForeignKey("dbo.ServiceRuleServiceItems", "ServiceRule_ServiceRuleID", "dbo.ServiceRules");
            DropForeignKey("dbo.ServiceRuleLogs", "ServiceRuleID", "dbo.ServiceRules");
            DropIndex("dbo.ServiceRuleServiceItems", new[] { "ServiceItem_ServiceItemID" });
            DropIndex("dbo.ServiceRuleServiceItems", new[] { "ServiceRule_ServiceRuleID" });
            DropIndex("dbo.ServiceRuleLogs", new[] { "ServiceRuleID" });
            DropTable("dbo.ServiceRuleServiceItems");
            DropTable("dbo.ServiceRuleLogs");
            DropTable("dbo.ServiceRules");
        }
    }
}
