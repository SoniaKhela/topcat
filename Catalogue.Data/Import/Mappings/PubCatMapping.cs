﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Catalogue.Data.Model;
using Catalogue.Gemini.Model;
using Catalogue.Utilities.Text;
using CsvHelper;
using CsvHelper.Configuration;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Document;

namespace Catalogue.Data.Import.Mappings
{
    public class PubCatMapping : IMapping
    {
        public IEnumerable<Vocabulary> RequiredVocabularies {
            get
            {
                return new List<Vocabulary>
                {
                    //only for empty test systems
                    Vocabularies.JnccCategory,
                    Vocabularies.JnccDomain,

                    new Vocabulary
                        {
                            Id = "http://vocab.jncc.gov.uk/publications",
                            Name = "Publication properties",
                            Description = "Describes various properties of JNCC Publications",
                            Controlled = true,
                            Publishable = true,
                            PublicationDate = "2015",
                            Keywords = new List<VocabularyKeyword>
                                {
                                    new VocabularyKeyword {Value = "Free"},
                                    new VocabularyKeyword {Value = "Discontinued"}
                                }

                        },
                        new Vocabulary
                        {
                            Id = "http://vocab.jncc.gov.uk/NHBS",
                            Name = "NHBS Numbers",
                            Description = "NHBS Number",
                            Controlled = false,
                            Publishable = true,
                            PublicationDate = "2015",
                            Keywords = new List<VocabularyKeyword>()
                        },
                        new Vocabulary
                        {
                            Id = "http://vocab.jncc.gov.uk/ISBN",
                            Name = "ISBN Numbers",
                            Description = "ISBN Numbers",
                            Controlled = false,
                            Publishable = true,
                            PublicationDate = "2015",
                            Keywords = new List<VocabularyKeyword>()
                        },
                        new Vocabulary
                        {
                            Id = "http://vocab.jncc.gov.uk/ISSN",
                            Name = "ISSN Numbers",
                            Description = "ISSN Numbers",
                            Controlled = false,
                            Publishable = true,
                            PublicationDate = "2015",
                            Keywords = new List<VocabularyKeyword>()
                        }
                };
            }

        }

        public void Apply(CsvConfiguration config)
        {
            config.Delimiter = "\t";
            config.QuoteAllFields = true;
            config.TrimFields = true;
            config.RegisterClassMap<RecordMap>();
            config.RegisterClassMap<GeminiMap>();
        }
    }

    public class GeminiMap : CsvClassMap<Metadata>
    {
        public override void CreateMap()
        {
            Map(m => m.Title).Name("Title");
            Map(m => m.ResponsibleOrganisation).ConvertUsing(row =>
                {
                    string name = row.GetField("Authors");
                    string email = String.Empty;
                    string role = "author";

                    return new ResponsibleParty {Name = name, Email = email, Role = role};
                });
            Map(m => m.Abstract).ConvertUsing(row =>
                {
                    var sb = new StringBuilder();
                    sb.AppendLine(row.GetField("ParsedPageContent"));
                    sb.AppendLine();
                    sb.AppendLine("## Citation");
                    sb.AppendLine();
                    sb.AppendLine(row.GetField("Citation"));
                    sb.AppendLine();
                    sb.AppendLine(row.GetField("Comment"));
                    sb.AppendLine();
                    sb.Append(GetBadKeywordText(row.GetField("Keywords")));

                    return sb.ToString();

                });
            //Invalid dates handled by exporter - go to comments field with note
            Map(m => m.DatasetReferenceDate).Name("PublicationDate");
            Map(m => m.Keywords).ConvertUsing(GetKeywords);
            Map(m => m.ResourceLocator).ConvertUsing(row => "http://some/example/public/location");
            Map(m => m.DataFormat).ConvertUsing(row => "Documents");
            Map(m => m.ResourceType).ConvertUsing(row => "publication");
        }

        private string GetBadKeywordText(string keywords)
        {

            var badKeywords = from k in ParsePageKeywords(keywords)
                              where k.Length > 30
                              select k;

            var enumerable = badKeywords as string[] ?? badKeywords.ToArray();
            if (!enumerable.Any()) return String.Empty;

            var output = new StringBuilder("##Bad Keywords");
            output.AppendLine();
            output.Append(
                "The following keywords were associated with the original CMS page for this publication. There length indicates they may be invalid");

            foreach (var badKeyword in enumerable)
            {
                output.AppendLine(badKeyword);
            }

            return output.ToString();
        }


        private List<MetadataKeyword> GetKeywords(ICsvReaderRow row)
        {
            var keywords = new List<MetadataKeyword>();
            
            keywords.AddRange(GetValidPageKeywords(row.GetField("Keywords")));

            AddKeyword(keywords, "http://vocab.jncc.gov.uk/NHBS",  row.GetField("NhbsNumber"));
            AddKeyword(keywords, "http://vocab.jncc.gov.uk/ISBN", row.GetField("IsbnNumber"));
            AddKeyword(keywords, "http://vocab.jncc.gov.uk/ISSN", row.GetField("IssnNumber"));
            AddKeyword(keywords, "http://vocab.jncc.gov.uk/JnccReportSeriesNumber", row.GetField("JnccReportSeriesNumber"));

            if (row.GetField("Free") == "1")
            {
                AddKeyword(keywords, "http://vocab.jncc.gov.uk/publications", "Free");
            }

            if (row.GetField("Discontinued") == "1")
            {
                AddKeyword(keywords, "http://vocab.jncc.gov.uk/publications", "Discontinued");
            }

            // not sure yet how to categorise publications
            AddKeyword(keywords, "http://vocab.jncc.gov.uk/jncc-domain", "to do!");
            AddKeyword(keywords, "http://vocab.jncc.gov.uk/jncc-category", "Publications");

            return keywords;
        }

        private void AddKeyword(List<MetadataKeyword> keywords, string vocab, string value)
        {
            if (value.IsBlank()) return;

            keywords.Add(new MetadataKeyword()
                {
                    Vocab = vocab,
                    Value = value
                });
        }

        private IEnumerable<string> ParsePageKeywords(string input)
        {

            if (input.IsBlank()) return new List<string>();

            return (from m in Regex.Matches(input, @"\{(.*?)\}").Cast<Match>()
             let keyword = m.Groups.Cast<Group>().Select(g => g.Value).Skip(1).First().Split(',')
             from k in keyword
             where k.IsNotBlank()
             select k).Distinct(StringComparer.InvariantCultureIgnoreCase);
        } 

        private IEnumerable<MetadataKeyword> GetValidPageKeywords(string input)
        {
            return from k in ParsePageKeywords(input)
                   where k.Length <= 30
                   select new MetadataKeyword
                       {
                           Vocab = String.Empty,
                           Value = k.Replace('"', ' ').Trim()
                       };
        }
    }

    public class RecordMap : CsvClassMap<Record>
    {
        public override void CreateMap()
        {
            Map(m => m.Path).Name("Path");
            Map(m => m.TopCopy).ConvertUsing(row => true);
            Map(m => m.Status).ConvertUsing(row  => Status.Publishable);

            References<GeminiMap>(m => m.Gemini);
        }
    }
    
    internal class import_runnner
    {
        [Explicit]
        [Test]
        public void RunPubcatImport()
        {
            var store = new DocumentStore();
            store.ParseConnectionString("Url=http://localhost:8888/");
            store.Initialize();

            using (IDocumentSession db = store.OpenSession())
            {
                var importer = Importer.CreateImporter<PubCatMapping>(db);
                importer.Import(@"C:\Working\pubcat.csv");
                //db.SaveChanges();
            }
        }
    }

}
