namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer9 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExportOrders",
                c => new
                    {
                        ExportOrderID = c.Int(nullable: false, identity: true),
                        Guid = c.String(),
                        ExportOrderNo = c.String(),
                        ExportTBID = c.String(),
                        CreateTime = c.DateTimeOffset(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.ExportOrderID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ExportOrders");
        }
    }
}
