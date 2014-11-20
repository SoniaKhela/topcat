using Raven.Imports.Newtonsoft.Json;

namespace Catalogue.Data.Model
{
    public class ResponsibleParty
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}