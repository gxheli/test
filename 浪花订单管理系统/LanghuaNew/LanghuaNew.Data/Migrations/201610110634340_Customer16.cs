namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer16 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerLogs",
                c => new
                    {
                        CustomerLogID = c.Int(nullable: false, identity: true),
                        CustomerID = c.Int(nullable: false),
                        OperID = c.String(),
                        OperName = c.String(),
                        OperTime = c.DateTimeOffset(nullable: false, precision: 7),
                        Remark = c.String(),
                        Operate = c.String(),
                    })
                .PrimaryKey(t => t.CustomerLogID)
                .ForeignKey("dbo.Customers", t => t.CustomerID, cascadeDelete: true)
                .Index(t => t.CustomerID);
            
            AddColumn("dbo.ServiceItemHistories", "ChangeTravelDate", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerLogs", "CustomerID", "dbo.Customers");
            DropIndex("dbo.CustomerLogs", new[] { "CustomerID" });
            DropColumn("dbo.ServiceItemHistories", "ChangeTravelDate");
            DropTable("dbo.CustomerLogs");
        }
    }
}
