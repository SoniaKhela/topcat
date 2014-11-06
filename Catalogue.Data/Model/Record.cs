using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Spatial;
using Catalogue.Gemini.Model;
using Raven.Imports.Newtonsoft.Json;

namespace Catalogue.Data.Model
{
    public class Record
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// The UK Gemini metadata record. 
        /// </summary>
        public virtual Metadata Gemini  { get; set; }

        public string   Path     { get; set; }
        public bool     TopCopy  { get; set; }
        public Status   Status   { get; set; }
        public Validation Validation { get; set; }
        public Security Security { get; set; }
        public DateTime Review   { get; set; }
        public string   Notes    { get; set; }

        /// <summary>
        /// An optional identifier for records imported from another source.
        /// </summary>
        public string SourceIdentifier { get; set; }
        public bool   ReadOnly         { get; set; } // for imported records

        /// <summary>
        /// A 'well known text' representation of the bounding box in the Gemini record
        /// used for spatial indexing.
        /// </summary>
        public string Wkt { get; set; }

        [JsonIgnore]
        public DbGeometry Geometry { get; set; }

        /// <summary>
        /// Used by the infrastructure when representing a particular revision of the record.
        /// </summary>
        public int Revision { get; internal set; }
    }
}
