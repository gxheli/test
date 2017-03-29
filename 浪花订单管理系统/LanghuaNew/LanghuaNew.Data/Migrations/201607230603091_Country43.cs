namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country43 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Travellers", "Customer_CustomerID", "dbo.Customers");
            DropIndex("dbo.Travellers", new[] { "Customer_CustomerID" });
            RenameColumn(table: "dbo.Travellers", name: "Customer_CustomerID", newName: "CustomerID");
            AlterColumn("dbo.Travellers", "CustomerID", c => c.Int(nullable: false));
            CreateIndex("dbo.Travellers", "CustomerID");
            AddForeignKey("dbo.Travellers", "CustomerID", "dbo.Customers", "CustomerID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Travellers", "CustomerID", "dbo.Customers");
            DropIndex("dbo.Travellers", new[] { "CustomerID" });
            AlterColumn("dbo.Travellers", "CustomerID", c => c.Int());
            RenameColumn(table: "dbo.Travellers", name: "CustomerID", newName: "Customer_CustomerID");
            CreateIndex("dbo.Travellers", "Customer_CustomerID");
            AddForeignKey("dbo.Travellers", "Customer_CustomerID", "dbo.Customers", "CustomerID");
        }
    }
}
