namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FormFields", "ExampleStyle", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FormFields", "ExampleStyle");
        }
    }
}
