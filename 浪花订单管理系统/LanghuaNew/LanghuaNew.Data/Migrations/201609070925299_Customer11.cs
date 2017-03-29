namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer11 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExtraSettings",
                c => new
                    {
                        ExtraSettingID = c.Int(nullable: false, identity: true),
                        SellControlID = c.Int(nullable: false),
                        StartTime = c.DateTimeOffset(nullable: false, precision: 7),
                        EndTime = c.DateTimeOffset(nullable: false, precision: 7),
                        ExtraSettingNum = c.Int(nullable: false),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.ExtraSettingID)
                .ForeignKey("dbo.SellControls", t => t.SellControlID, cascadeDelete: true)
                .Index(t => t.SellControlID);
            
            CreateTable(
                "dbo.BillReports",
                c => new
                    {
                        BillReportID = c.Int(nullable: false, identity: true),
                        SupplierID = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                        StartDate = c.DateTimeOffset(nullable: false, precision: 7),
                        EndDate = c.DateTimeOffset(nullable: false, precision: 7),
                        TotalReceive = c.Single(nullable: false),
                        CreateTime = c.DateTimeOffset(nullable: false, precision: 7),
                        PayTime = c.DateTimeOffset(nullable: false, precision: 7),
                        Remark = c.String(),
                        FileStream = c.String(),
                    })
                .PrimaryKey(t => t.BillReportID)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID, cascadeDelete: true)
                .Index(t => t.SupplierID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BillReports", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.ExtraSettings", "SellControlID", "dbo.SellControls");
            DropIndex("dbo.BillReports", new[] { "SupplierID" });
            DropIndex("dbo.ExtraSettings", new[] { "SellControlID" });
            DropTable("dbo.BillReports");
            DropTable("dbo.ExtraSettings");
        }
    }
}
