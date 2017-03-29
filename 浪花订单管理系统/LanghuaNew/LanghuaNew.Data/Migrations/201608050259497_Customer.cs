namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerReturnLists",
                c => new
                    {
                        CustomerReturnListID = c.Int(nullable: false, identity: true),
                        ServiceItemID = c.Int(nullable: false),
                        SupplierID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CustomerReturnListID)
                .ForeignKey("dbo.ServiceItems", t => t.ServiceItemID, cascadeDelete: true)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID, cascadeDelete: true)
                .Index(t => t.ServiceItemID)
                .Index(t => t.SupplierID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CustomerReturnLists", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.CustomerReturnLists", "ServiceItemID", "dbo.ServiceItems");
            DropIndex("dbo.CustomerReturnLists", new[] { "SupplierID" });
            DropIndex("dbo.CustomerReturnLists", new[] { "ServiceItemID" });
            DropTable("dbo.CustomerReturnLists");
        }
    }
}
