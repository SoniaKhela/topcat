namespace Catalogue.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTestTable : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.Cows");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Cows",
                c => new
                    {
                        CowId = c.Int(nullable: false, identity: true),
                        DynamicMoooWidget = c.String(),
                    })
                .PrimaryKey(t => t.CowId);
            
        }
    }
}
