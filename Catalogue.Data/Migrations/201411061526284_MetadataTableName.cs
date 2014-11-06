namespace Catalogue.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MetadataTableName : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Metadatas", newName: "Metadata");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.Metadata", newName: "Metadatas");
        }
    }
}
