namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer12 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillReports", "State", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.BillReports", "State");
        }
    }
}
