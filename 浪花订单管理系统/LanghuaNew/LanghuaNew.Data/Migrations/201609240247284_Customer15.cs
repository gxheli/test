namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer15 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OrderSupplierHistories",
                c => new
                    {
                        OrderSupplierHistoryID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        OperUserID = c.Int(nullable: false),
                        OperNickName = c.String(),
                        opera = c.Int(nullable: false),
                        OperTime = c.DateTimeOffset(nullable: false, precision: 7),
                        Remark = c.String(),
                        State = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OrderSupplierHistoryID)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .Index(t => t.OrderID);
            
            AddColumn("dbo.Orders", "isUrgent", c => c.Boolean(nullable: false));
            AddColumn("dbo.ServiceRules", "Remark", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderSupplierHistories", "OrderID", "dbo.Orders");
            DropIndex("dbo.OrderSupplierHistories", new[] { "OrderID" });
            DropColumn("dbo.ServiceRules", "Remark");
            DropColumn("dbo.Orders", "isUrgent");
            DropTable("dbo.OrderSupplierHistories");
        }
    }
}
