namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country12 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ServiceItems", "ServiceItemTemplteID", "dbo.ServiceItemTempltes");
            DropForeignKey("dbo.ServiceItemHistories", "ServiceItemTemplteID", "dbo.ServiceItemTempltes");
            DropIndex("dbo.ServiceItems", new[] { "ServiceItemTemplteID" });
            DropIndex("dbo.ServiceItemHistories", new[] { "ServiceItemTemplteID" });
            AlterColumn("dbo.ServiceItems", "ServiceItemTemplteID", c => c.Int());
            AlterColumn("dbo.ServiceItemHistories", "ServiceItemTemplteID", c => c.Int());
                                                                                                                                CreateIndex("dbo.ServiceItems", "ServiceItemTemplteID");
            CreateIndex("dbo.ServiceItemHistories", "ServiceItemTemplteID");
            AddForeignKey("dbo.ServiceItems", "ServiceItemTemplteID", "dbo.ServiceItemTempltes", "ServiceItemTemplteID");
            AddForeignKey("dbo.ServiceItemHistories", "ServiceItemTemplteID", "dbo.ServiceItemTempltes", "ServiceItemTemplteID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceItemHistories", "ServiceItemTemplteID", "dbo.ServiceItemTempltes");
            DropForeignKey("dbo.ServiceItems", "ServiceItemTemplteID", "dbo.ServiceItemTempltes");
            DropIndex("dbo.ServiceItemHistories", new[] { "ServiceItemTemplteID" });
            DropIndex("dbo.ServiceItems", new[] { "ServiceItemTemplteID" });
            AlterColumn("dbo.ServiceItemHistories", "ServiceItemTemplteID", c => c.Int(nullable: false));
            AlterColumn("dbo.ServiceItems", "ServiceItemTemplteID", c => c.Int(nullable: false));
            CreateIndex("dbo.ServiceItemHistories", "ServiceItemTemplteID");
            CreateIndex("dbo.ServiceItems", "ServiceItemTemplteID");
            AddForeignKey("dbo.ServiceItemHistories", "ServiceItemTemplteID", "dbo.ServiceItemTempltes", "ServiceItemTemplteID", cascadeDelete: true);
            AddForeignKey("dbo.ServiceItems", "ServiceItemTemplteID", "dbo.ServiceItemTempltes", "ServiceItemTemplteID", cascadeDelete: true);
        }
    }
}
