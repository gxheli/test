namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country24 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AlipayTransfers", "ReceiveName", c => c.String());
            AddColumn("dbo.TBOrderStates", "OrderSourseID", c => c.Int(nullable: false));
            AddColumn("dbo.TBOrderStates", "OrderDate", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.AlipayTransferLogs", "UserName", c => c.String());
            CreateIndex("dbo.TBOrderStates", "OrderSourseID");
            AddForeignKey("dbo.TBOrderStates", "OrderSourseID", "dbo.OrderSourses", "OrderSourseID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TBOrderStates", "OrderSourseID", "dbo.OrderSourses");
            DropIndex("dbo.TBOrderStates", new[] { "OrderSourseID" });
            AlterColumn("dbo.AlipayTransferLogs", "UserName", c => c.Int(nullable: false));
            DropColumn("dbo.TBOrderStates", "OrderDate");
            DropColumn("dbo.TBOrderStates", "OrderSourseID");
            DropColumn("dbo.AlipayTransfers", "ReceiveName");
        }
    }
}
