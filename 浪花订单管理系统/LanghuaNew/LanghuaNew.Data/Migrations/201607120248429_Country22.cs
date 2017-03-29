namespace LanghuaNew.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Country22 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FormFields",
                c => new
                    {
                        FormFieldID = c.Int(nullable: false, identity: true),
                        Key = c.String(),
                        FieldName = c.String(),
                        FieldNo = c.String(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.FormFieldID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FormFields");
        }
    }
}
