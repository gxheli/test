namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country18 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ServiceRuleServiceItems", "ServiceRule_ServiceRuleID", "dbo.ServiceRules");
            DropIndex("dbo.ServiceRuleServiceItems", new[] { "ServiceRule_ServiceRuleID" });
            DropPrimaryKey("dbo.ServiceRules");
            DropPrimaryKey("dbo.ServiceRuleServiceItems");
            AddColumn("dbo.ServiceItemHistories", "ServiceItemTemplteValue", c => c.String());
            AlterColumn("dbo.ServiceRules", "ServiceRuleID", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.ServiceRuleServiceItems", "ServiceRule_ServiceRuleID", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.ServiceRules", "ServiceRuleID");
            AddPrimaryKey("dbo.ServiceRuleServiceItems", new[] { "ServiceRule_ServiceRuleID", "ServiceItem_ServiceItemID" });
            CreateIndex("dbo.ServiceRuleServiceItems", "ServiceRule_ServiceRuleID");
            AddForeignKey("dbo.ServiceRuleServiceItems", "ServiceRule_ServiceRuleID", "dbo.ServiceRules", "ServiceRuleID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceRuleServiceItems", "ServiceRule_ServiceRuleID", "dbo.ServiceRules");
            DropIndex("dbo.ServiceRuleServiceItems", new[] { "ServiceRule_ServiceRuleID" });
            DropPrimaryKey("dbo.ServiceRuleServiceItems");
            DropPrimaryKey("dbo.ServiceRules");
            AlterColumn("dbo.ServiceRuleServiceItems", "ServiceRule_ServiceRuleID", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ServiceRules", "ServiceRuleID", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.ServiceItemHistories", "ServiceItemTemplteValue");
            AddPrimaryKey("dbo.ServiceRuleServiceItems", new[] { "ServiceRule_ServiceRuleID", "ServiceItem_ServiceItemID" });
            AddPrimaryKey("dbo.ServiceRules", "ServiceRuleID");
            CreateIndex("dbo.ServiceRuleServiceItems", "ServiceRule_ServiceRuleID");
            AddForeignKey("dbo.ServiceRuleServiceItems", "ServiceRule_ServiceRuleID", "dbo.ServiceRules", "ServiceRuleID", cascadeDelete: true);
        }
    }
}
