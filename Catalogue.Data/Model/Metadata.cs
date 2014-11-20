using System;
using System.Collections.Generic;
using Raven.Imports.Newtonsoft.Json;

namespace Catalogue.Data.Model
{
    /// <summary>
    /// A simple implementation of the UK Gemini 2.2 metadata standard.
    /// </summary>
    public class Metadata
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string TopicCategory { get; set; }
        public virtual List<Keyword> Keywords { get; set; }
        public virtual TemporalExtent TemporalExtent { get; set; }
        public DateTime DatasetReferenceDate { get; set; } // / this should be changed to a collection for creation/publication/revision
        public SupportedLanguage DatasetLanguage { get; set; }
        public string Lineage { get; set; }
//      public decimal SpatialResolution { get; set; } // todo we'll probably need this https://wiki.ceh.ac.uk/display/cehigh/Spatial+resolution
        public string ResourceLocator { get; set; }
        public string AdditionalInformationSource { get; set; }
        public string DataFormat { get; set; } // mesh uses MEDIN data format categories from http://vocab.ndg.nerc.ac.uk/client/vocabServer.jsp
        public virtual ResponsibleParty ResponsibleOrganisation { get; set; }
        public string LimitationsOnPublicAccess { get; set; }
        public string UseConstraints { get; set; }
        public string SpatialReferenceSystem { get; set; }
        public virtual List<Extent> Extent { get; set; } // support multiple locations; use same UI as keywords
        public DateTime MetadataDate { get; set; }
        public SupportedLanguage MetadataLanguage { get; set; }
        
        public virtual ResponsibleParty MetadataPointOfContact { get; set; }
        public string ResourceType { get; set; }  // dataset | series | service
        public virtual BoundingBox BoundingBox { get; set; }

        public Metadata()
        {
            Keywords = new List<Keyword>();
            TemporalExtent = new TemporalExtent();
            Extent = new List<Extent>();
            ResponsibleOrganisation = new ResponsibleParty();
            MetadataPointOfContact = new ResponsibleParty();
            BoundingBox = new BoundingBox();
        }
    }

    /// <summary>
    /// lowercase to match gemini spec
    /// </summary>
    public enum SupportedLanguage
    {
        eng = 1,
        deu =2,
        fra =3,
        fin = 4
    }

    public class Extent
    {
        [JsonIgnore]
        public int  ExtentId { get; set; }
        public string Value { get; set; }
        public string Authority { get; set; }
    }

    public class TemporalExtent
    {
        [JsonIgnore]
        public int TemporalExtentId { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
    }

    public class ResponsibleParty
    {
        [JsonIgnore]
        public int ResponsiblePartyId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    /// <summary>
    ///  Bounding box referenced to WGS 84.
    /// </summary>
    public class BoundingBox
    {
        [JsonIgnore]
        public int BoundingBoxId { get; set; }
        public decimal North { get; set; }
        public decimal South { get; set; }
        public decimal East { get; set; }
        public decimal West { get; set; }
    }

}
