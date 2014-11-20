using Raven.Imports.Newtonsoft.Json;

namespace Catalogue.Data.Model
{
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