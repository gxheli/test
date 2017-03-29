namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country35 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ServiceRules", "Week", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ServiceRules", "Week", c => c.Int(nullable: false));
        }
    }
}
