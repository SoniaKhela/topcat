namespace Catalogue.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cows",
                c => new
                    {
                        CowId = c.Int(nullable: false, identity: true),
                        DynamicMoooWidget = c.String(),
                    })
                .PrimaryKey(t => t.CowId);
            
            CreateTable(
                "dbo.Extents",
                c => new
                    {
                        ExtentId = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        Authority = c.String(),
                        Metadata_MetadataId = c.Int(),
                    })
                .PrimaryKey(t => t.ExtentId)
                .ForeignKey("dbo.Metadatas", t => t.Metadata_MetadataId)
                .Index(t => t.Metadata_MetadataId);
            
            CreateTable(
                "dbo.Metadatas",
                c => new
                    {
                        MetadataId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Abstract = c.String(),
                        TopicCategory = c.String(),
                        DatasetReferenceDate = c.DateTime(nullable: false),
                        DatasetLanguage = c.Int(nullable: false),
                        Lineage = c.String(),
                        ResourceLocator = c.String(),
                        AdditionalInformationSource = c.String(),
                        DataFormat = c.String(),
                        LimitationsOnPublicAccess = c.String(),
                        UseConstraints = c.String(),
                        SpatialReferenceSystem = c.String(),
                        MetadataDate = c.DateTime(nullable: false),
                        MetadataLanguage = c.Int(nullable: false),
                        ResourceType = c.String(),
                        BoundingBox_BoundingBoxId = c.Int(),
                        MetadataPointOfContact_ResponsiblePartyId = c.Int(),
                        ResponsibleOrganisation_ResponsiblePartyId = c.Int(),
                        TemporalExtent_TemporalExtentId = c.Int(),
                    })
                .PrimaryKey(t => t.MetadataId)
                .ForeignKey("dbo.BoundingBoxes", t => t.BoundingBox_BoundingBoxId)
                .ForeignKey("dbo.ResponsibleParties", t => t.MetadataPointOfContact_ResponsiblePartyId)
                .ForeignKey("dbo.ResponsibleParties", t => t.ResponsibleOrganisation_ResponsiblePartyId)
                .ForeignKey("dbo.TemporalExtents", t => t.TemporalExtent_TemporalExtentId)
                .Index(t => t.BoundingBox_BoundingBoxId)
                .Index(t => t.MetadataPointOfContact_ResponsiblePartyId)
                .Index(t => t.ResponsibleOrganisation_ResponsiblePartyId)
                .Index(t => t.TemporalExtent_TemporalExtentId);
            
            CreateTable(
                "dbo.BoundingBoxes",
                c => new
                    {
                        BoundingBoxId = c.Int(nullable: false, identity: true),
                        North = c.Decimal(nullable: false, precision: 18, scale: 2),
                        South = c.Decimal(nullable: false, precision: 18, scale: 2),
                        East = c.Decimal(nullable: false, precision: 18, scale: 2),
                        West = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.BoundingBoxId);
            
            CreateTable(
                "dbo.MetadataKeywords",
                c => new
                    {
                        MetadataKeywordId = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        Vocab = c.String(),
                        Metadata_MetadataId = c.Int(),
                    })
                .PrimaryKey(t => t.MetadataKeywordId)
                .ForeignKey("dbo.Metadatas", t => t.Metadata_MetadataId)
                .Index(t => t.Metadata_MetadataId);
            
            CreateTable(
                "dbo.ResponsibleParties",
                c => new
                    {
                        ResponsiblePartyId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Email = c.String(),
                        Role = c.String(),
                    })
                .PrimaryKey(t => t.ResponsiblePartyId);
            
            CreateTable(
                "dbo.TemporalExtents",
                c => new
                    {
                        TemporalExtentId = c.Int(nullable: false, identity: true),
                        Begin = c.DateTime(nullable: false),
                        End = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.TemporalExtentId);
            
            CreateTable(
                "dbo.Records",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Path = c.String(),
                        TopCopy = c.Boolean(nullable: false),
                        Status = c.Int(nullable: false),
                        Validation = c.Int(nullable: false),
                        Security = c.Int(nullable: false),
                        Review = c.DateTime(nullable: false),
                        Notes = c.String(),
                        SourceIdentifier = c.String(),
                        ReadOnly = c.Boolean(nullable: false),
                        Wkt = c.String(),
                        Geometry = c.Geometry(),
                        Revision = c.Int(nullable: false),
                        Gemini_MetadataId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Metadatas", t => t.Gemini_MetadataId)
                .Index(t => t.Gemini_MetadataId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Records", "Gemini_MetadataId", "dbo.Metadatas");
            DropForeignKey("dbo.Metadatas", "TemporalExtent_TemporalExtentId", "dbo.TemporalExtents");
            DropForeignKey("dbo.Metadatas", "ResponsibleOrganisation_ResponsiblePartyId", "dbo.ResponsibleParties");
            DropForeignKey("dbo.Metadatas", "MetadataPointOfContact_ResponsiblePartyId", "dbo.ResponsibleParties");
            DropForeignKey("dbo.MetadataKeywords", "Metadata_MetadataId", "dbo.Metadatas");
            DropForeignKey("dbo.Extents", "Metadata_MetadataId", "dbo.Metadatas");
            DropForeignKey("dbo.Metadatas", "BoundingBox_BoundingBoxId", "dbo.BoundingBoxes");
            DropIndex("dbo.Records", new[] { "Gemini_MetadataId" });
            DropIndex("dbo.MetadataKeywords", new[] { "Metadata_MetadataId" });
            DropIndex("dbo.Metadatas", new[] { "TemporalExtent_TemporalExtentId" });
            DropIndex("dbo.Metadatas", new[] { "ResponsibleOrganisation_ResponsiblePartyId" });
            DropIndex("dbo.Metadatas", new[] { "MetadataPointOfContact_ResponsiblePartyId" });
            DropIndex("dbo.Metadatas", new[] { "BoundingBox_BoundingBoxId" });
            DropIndex("dbo.Extents", new[] { "Metadata_MetadataId" });
            DropTable("dbo.Records");
            DropTable("dbo.TemporalExtents");
            DropTable("dbo.ResponsibleParties");
            DropTable("dbo.MetadataKeywords");
            DropTable("dbo.BoundingBoxes");
            DropTable("dbo.Metadatas");
            DropTable("dbo.Extents");
            DropTable("dbo.Cows");
        }
    }
}
