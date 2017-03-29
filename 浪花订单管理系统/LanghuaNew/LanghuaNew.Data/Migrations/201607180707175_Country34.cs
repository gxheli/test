namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country34 : DbMigration
    {
        public override void Up()
        {
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceRuleLogs", "ServiceRuleID", "dbo.ServiceRules");
            DropIndex("dbo.ServiceRuleLogs", new[] { "ServiceRuleID" });
            DropTable("dbo.ServiceRuleLogs");
        }
    }
}
