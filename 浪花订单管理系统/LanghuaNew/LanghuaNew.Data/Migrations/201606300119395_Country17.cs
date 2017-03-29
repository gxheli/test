namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country17 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ExtraServicePrices", "Service_ExtraServiceID", "dbo.ExtraServices");
            DropIndex("dbo.ExtraServicePrices", new[] { "Service_ExtraServiceID" });
            RenameColumn(table: "dbo.ExtraServicePrices", name: "Service_ExtraServiceID", newName: "ExtraServiceID");
            AlterColumn("dbo.ExtraServicePrices", "ExtraServiceID", c => c.Int(nullable: false));
            CreateIndex("dbo.ExtraServicePrices", "ExtraServiceID");
            AddForeignKey("dbo.ExtraServicePrices", "ExtraServiceID", "dbo.ExtraServices", "ExtraServiceID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExtraServicePrices", "ExtraServiceID", "dbo.ExtraServices");
            DropIndex("dbo.ExtraServicePrices", new[] { "ExtraServiceID" });
            AlterColumn("dbo.ExtraServicePrices", "ExtraServiceID", c => c.Int());
            RenameColumn(table: "dbo.ExtraServicePrices", name: "ExtraServiceID", newName: "Service_ExtraServiceID");
            CreateIndex("dbo.ExtraServicePrices", "Service_ExtraServiceID");
            AddForeignKey("dbo.ExtraServicePrices", "Service_ExtraServiceID", "dbo.ExtraServices", "ExtraServiceID");
        }
    }
}
