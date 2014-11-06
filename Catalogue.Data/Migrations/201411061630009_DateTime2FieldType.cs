namespace Catalogue.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateTime2FieldType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Metadata", "DatasetReferenceDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Metadata", "MetadataDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TemporalExtents", "Begin", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.TemporalExtents", "End", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.Records", "Review", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Records", "Review", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TemporalExtents", "End", c => c.DateTime(nullable: false));
            AlterColumn("dbo.TemporalExtents", "Begin", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Metadata", "MetadataDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Metadata", "DatasetReferenceDate", c => c.DateTime(nullable: false));
        }
    }
}
