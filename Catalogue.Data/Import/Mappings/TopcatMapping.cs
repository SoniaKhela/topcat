using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalogue.Data.Export;
using Catalogue.Data.Model;
using Catalogue.Data.Seed;
using Catalogue.Gemini.Model;
using Catalogue.Utilities.Text;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Newtonsoft.Json;

namespace Catalogue.Data.Import.Mappings
{
    /// <summary>
    /// Supports a standard Topcat tab-separated record format (as created by the exporter).
    /// </summary>
    public class TopcatMapping : IMapping
    {
        public IEnumerable<Vocabulary> RequiredVocabularies
        {
            get
            {
                return new List<Vocabulary>
                {
                    Vocabularies.JnccCategory,
                    Vocabularies.JnccDomain,
                };
            }
        }
        
        public void Apply(CsvConfiguration config)
        {
            ApplyStandardTopcatCsvConfiguration(config);
        }

        public static CsvConfiguration ApplyStandardTopcatCsvConfiguration(CsvConfiguration config)
        {
            config.Delimiter = "\t";

            config.RegisterClassMap<RecordMap>();
            config.RegisterClassMap<GeminiMap>();

            config.WillThrowOnMissingField = false;
            //config.PrefixReferenceHeaders = true;
            TypeConverterFactory.AddConverter<List<MetadataKeyword>>(new MetadataKeywordConverter());
            TypeConverterFactory.AddConverter<List<Extent>>(new ExtentListConverter());

            return config;
        }


        public sealed class RecordMap : CsvClassMap<Record>
        {
            public RecordMap()
            {
                Map(m => m.Path);
                Map(m => m.TopCopy);
                Map(m => m.Status);
                Map(m => m.Security);
                Map(m => m.Review);
                Map(m => m.Notes);
                Map(m => m.SourceIdentifier);
                Map(m => m.ReadOnly);

                References<GeminiMap>(m => m.Gemini);
            }
        }

        public sealed class GeminiMap : CsvClassMap<Metadata>
        {
            public GeminiMap()
            {
                Map(m => m.Title).Name("Gemini.Title");
                Map(m => m.Abstract).Name("Gemini.Abstract");
                Map(m => m.TopicCategory).Name("Gemini.TopicCategory");
                Map(m => m.Keywords).Name("Gemini.Keywords");
                Map(m => m.TemporalExtent).Name("Gemini.TemporalExtent");
                Map(m => m.DatasetReferenceDate).Name("DatasetReferenceDate");
                Map(m => m.Lineage).Name("Gemini.Lineage");
                Map(m => m.ResourceLocator).Name("ResourceLocator");
                Map(m => m.AdditionalInformationSource).Name("AdditionalInformationSource");
                Map(m => m.DataFormat).Name("Gemini.DataFormat");
                Map(m => m.ResponsibleOrganisation).Name("ResponsibleOrganisation");
                Map(m => m.LimitationsOnPublicAccess).Name("Gemini.LimitationsOnPublicAccess");
                Map(m => m.UseConstraints).Name("Gemini.UseConstraints");
                Map(m => m.SpatialReferenceSystem).Name("Gemini.SpatialReferenceSystem");
                //Map(m => m.Extent).Name("Gemini.Extent");
                Map(m => m.MetadataDate).Name("MetadataDate");
                Map(m => m.MetadataPointOfContact).Name("MetadataPointOfContact");
                Map(m => m.ResourceType).Name("Gemini.ResourceType");
                Map(m => m.BoundingBox).Name("Gemini.BoundingBox");
            }
        }


        public class MetadataKeywordConverter : DefaultTypeConverter
        {
            public override bool CanConvertTo(Type type)
            {
                return type == typeof(string);
            }

            public override bool CanConvertFrom(Type type)
            {
                return type == typeof(string);
            }

            public override string ConvertToString(TypeConverterOptions options, object value)
            {
                var v = (List<MetadataKeyword>)value;
                return JsonConvert.SerializeObject(v);
            }

            public override object ConvertFromString(TypeConverterOptions options, string text)
            {
                return JsonConvert.DeserializeObject(text, typeof(List<MetadataKeyword>));
            }
        }

        public class ExtentListConverter : DefaultTypeConverter
        {
            public override bool CanConvertTo(Type type)
            {
                return type == typeof(string);
            }

            public override string ConvertToString(TypeConverterOptions options, object value)
            {
                var v = (List<Extent>)value;
                return JsonConvert.SerializeObject(v);
            }
        }

    }
}
