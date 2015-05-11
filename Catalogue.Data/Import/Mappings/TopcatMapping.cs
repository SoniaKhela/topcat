using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalogue.Data.Export;
using Catalogue.Gemini.Model;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace Catalogue.Data.Import.Mappings
{
    /// <summary>
    /// Supports the standard Topcat tab-separated record format (as created by the exporter).
    /// </summary>
    public class TopcatMapping : IMapping
    {
        public IEnumerable<Vocabulary> RequiredVocabularies
        {
            get
            {
                return new List<Vocabulary>
                {
                    //only for empty test systems
                    Vocabularies.JnccCategory,
                    Vocabularies.JnccDomain,
                };
            }
        }

        public void Apply(CsvConfiguration config)
        {
            config.Delimiter = "\t";
            config.PrefixReferenceHeaders = true;
            TypeConverterFactory.AddConverter<List<MetadataKeyword>>(new Exporter.MetadataKeywordConverter());
            TypeConverterFactory.AddConverter<List<Extent>>(new Exporter.ExtentListConverter());

            // do nothing! for this import mapping we're going to use defaults
        }
    }
}
