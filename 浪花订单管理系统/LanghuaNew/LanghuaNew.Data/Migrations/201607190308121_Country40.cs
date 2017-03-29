namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country40 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceRules", "RuleUseTypeVule", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceRules", "RuleUseTypeVule");
        }
    }
}
