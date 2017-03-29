namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country41 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceRules", "RuleUseTypeValue", c => c.Int(nullable: false));
            DropColumn("dbo.ServiceRules", "RuleUseTypeVule");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ServiceRules", "RuleUseTypeVule", c => c.Int(nullable: false));
            DropColumn("dbo.ServiceRules", "RuleUseTypeValue");
        }
    }
}
