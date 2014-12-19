using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Catalogue.Data.Model
{

    public class Vocabulary
    {
        [MaxLength(450)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PublicationDate { get; set; }

        public bool Controlled { get; set; } // is this a controlled (limited) list or are new values allowed?
        public bool Publishable { get; set; } // is this a vocabulary of publishable keywords?
     
        public virtual List<Keyword> Keywords { get; set; }
    }
}
