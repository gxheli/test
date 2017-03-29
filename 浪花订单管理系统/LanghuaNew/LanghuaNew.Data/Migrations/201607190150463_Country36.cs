namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country36 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ServiceRules", "ServiceRuleID", c => c.Int(nullable: false, identity: true));
        }
        
        public override void Down()
        {
        }
    }
}
