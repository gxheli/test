namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Customer24 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupplierServiceItems", "Remark", c => c.String());
            AddColumn("dbo.SupplierServiceItemChanges", "Remark", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SupplierServiceItemChanges", "Remark");
            DropColumn("dbo.SupplierServiceItems", "Remark");
        }
    }
}
