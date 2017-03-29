namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer13 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillReports", "FilePath", c => c.String());
            AlterColumn("dbo.BillReports", "TotalReceive", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BillReports", "TotalReceive", c => c.Single(nullable: false));
            DropColumn("dbo.BillReports", "FilePath");
        }
    }
}
