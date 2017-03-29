namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country11 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServiceItemTempltes",
                c => new
                    {
                        ServiceItemTemplteID = c.Int(nullable: false, identity: true),
                        ServiceItemTemplteHtml = c.String(),
                    })
                .PrimaryKey(t => t.ServiceItemTemplteID);
            
            AddColumn("dbo.ServiceItems", "ServiceItemTemplteID", c => c.Int(nullable:true));
            AddColumn("dbo.ServiceItemHistories", "ServiceTypeID", c => c.Int(nullable: false));
            AddColumn("dbo.ServiceItemHistories", "ServiceTypeName", c => c.String());
            AddColumn("dbo.ServiceItemHistories", "ServiceItemTemplteID", c => c.Int(nullable: true));
            CreateIndex("dbo.ServiceItems", "ServiceItemTemplteID");
            CreateIndex("dbo.ServiceItemHistories", "ServiceItemTemplteID");
            AddForeignKey("dbo.ServiceItems", "ServiceItemTemplteID", "dbo.ServiceItemTempltes", "ServiceItemTemplteID", cascadeDelete: false);
            AddForeignKey("dbo.ServiceItemHistories", "ServiceItemTemplteID", "dbo.ServiceItemTempltes", "ServiceItemTemplteID", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceItemHistories", "ServiceItemTemplteID", "dbo.ServiceItemTempltes");
            DropForeignKey("dbo.ServiceItems", "ServiceItemTemplteID", "dbo.ServiceItemTempltes");
            DropIndex("dbo.ServiceItemHistories", new[] { "ServiceItemTemplteID" });
            DropIndex("dbo.ServiceItems", new[] { "ServiceItemTemplteID" });
            DropColumn("dbo.ServiceItemHistories", "ServiceItemTemplteID");
            DropColumn("dbo.ServiceItemHistories", "ServiceTypeName");
            DropColumn("dbo.ServiceItemHistories", "ServiceTypeID");
            DropColumn("dbo.ServiceItems", "ServiceItemTemplteID");
            DropTable("dbo.ServiceItemTempltes");
        }
    }
}
