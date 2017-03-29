namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer33 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TBOrderNoes",
                c => new
                    {
                        TBOrderNoID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        No = c.String(),
                        SubNo = c.String(),
                    })
                .PrimaryKey(t => t.TBOrderNoID)
                .ForeignKey("dbo.Orders", t => t.OrderID, cascadeDelete: true)
                .Index(t => t.OrderID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TBOrderNoes", "OrderID", "dbo.Orders");
            DropIndex("dbo.TBOrderNoes", new[] { "OrderID" });
            DropTable("dbo.TBOrderNoes");
        }
    }
}
