namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country45 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceItemHistories", "ChangeElementsValue", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceItemHistories", "ChangeElementsValue");
        }
    }
}
