namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country33 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceRules", "UseDate", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceRules", "UseDate");
        }
    }
}
