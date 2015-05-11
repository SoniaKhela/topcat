using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalogue.Gemini.Model;

namespace Catalogue.Data.Import
{
    public static class Vocabularies
    {
        public static Vocabulary JnccDomain
        {
            get
            {
                return new Vocabulary
                {
                    Id = "http://vocab.jncc.gov.uk/jncc-domain",
                    Name = "JNCC Domain",
                    Description = "Groups metadata records into broad areas.",
                    PublicationDate = "2015",
                    Publishable = true,
                    Controlled = true,
                    Keywords = new List<VocabularyKeyword>
                        {
                            new VocabularyKeyword { Value = "Marine" },
                            new VocabularyKeyword { Value = "Freshwater" },
                            new VocabularyKeyword { Value = "Terrestrial" },
                            new VocabularyKeyword { Value = "Atmosphere" },
                        }
                };
            }
        }

        public static Vocabulary JnccCategory
        {
            get
            {
                return new Vocabulary
                {
                    Id = "http://vocab.jncc.gov.uk/jncc-category",
                    Name = "JNCC Category",
                    Description = "Groups metadata records into collections.",
                    PublicationDate = "2015",
                    Publishable = true,
                    Controlled = true,
                    Keywords = new List<VocabularyKeyword>
                        {
                            new VocabularyKeyword { Value = "Seabed Habitat Maps" },
                            new VocabularyKeyword { Value = "Human Activities" },
                            new VocabularyKeyword { Value = "Publications" },
                        }
                };
            }
        }
    }
}
