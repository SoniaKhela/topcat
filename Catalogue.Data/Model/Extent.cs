using Raven.Imports.Newtonsoft.Json;

namespace Catalogue.Data.Model
{
    public class Extent
    {
        [JsonIgnore]
        public int  Id { get; set; }
        public string Value { get; set; }
        public string Authority { get; set; }
    }
}