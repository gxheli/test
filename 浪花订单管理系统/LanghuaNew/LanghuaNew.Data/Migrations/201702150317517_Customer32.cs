namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer32 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Suppliers", "SupplierEnName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Suppliers", "SupplierEnName");
        }
    }
}
