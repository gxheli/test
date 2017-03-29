namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country10 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ServiceItems", "CreateTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.ServiceRules", "StartTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.ServiceRules", "EndTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.ServiceRules", "RangeStart", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.ServiceRules", "RangeEnd", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.ItemPriceBySuppliers", "startTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.ItemPriceBySuppliers", "EndTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Orders", "OrderDate", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Orders", "CreateTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.OrderHistories", "OperTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.ServiceItemHistories", "TravelDate", c => c.DateTimeOffset(nullable: false, precision: 7));
           
            AlterColumn("dbo.ServiceItemHistories", "ReturnDate", c => c.DateTimeOffset(nullable: false, precision: 7,defaultValue:DateTimeOffset.Now));
            AlterColumn("dbo.ServiceItemHistories", "CreateTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Travellers", "Birthday", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Travellers", "CreateTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Roles", "CreateTime", c => c.DateTimeOffset(nullable: false, precision: 7));
            AlterColumn("dbo.Users", "LateOnLineTime", c => c.DateTimeOffset(nullable: false, precision: 7));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "LateOnLineTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Roles", "CreateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Travellers", "CreateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Travellers", "Birthday", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ServiceItemHistories", "CreateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ServiceItemHistories", "ReturnDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ServiceItemHistories", "TravelDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.OrderHistories", "OperTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Orders", "CreateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Orders", "OrderDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ItemPriceBySuppliers", "EndTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ItemPriceBySuppliers", "startTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ServiceRules", "RangeEnd", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ServiceRules", "RangeStart", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ServiceRules", "EndTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ServiceRules", "StartTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ServiceItems", "CreateTime", c => c.DateTime(nullable: false));
        }
    }
}
