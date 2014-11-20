using System;
using Raven.Imports.Newtonsoft.Json;

namespace Catalogue.Data.Model
{
    public class TemporalExtent
    {
        [JsonIgnore]
        public int Id { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
    }
}