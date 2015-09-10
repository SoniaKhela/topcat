using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalogue.Data.Export;
using Catalogue.Data.Import;
using Catalogue.Data.Import.Mappings;
using Catalogue.Data.Model;
using Catalogue.Gemini.Model;
using Catalogue.Gemini.Templates;
using Catalogue.Utilities.Clone;
using CsvHelper;
using FluentAssertions;
using NUnit.Framework;
using Raven.Imports.Newtonsoft.Json;

namespace Catalogue.Tests.Slow.Catalogue.Data.Import
{
    class topcat_mapping_tests : DatabaseTestFixture
    {
        [Test]
        public void blah()
        {
            var records = new [] { ExampleRecord() };
            var writer = new StringWriter();

            try
            {
            new Exporter(writer).ExportRecord(ExampleRecord());
            }
            catch (CsvHelperException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.Data["CsvHelper"]);
            }

            string exported = writer.ToString();

            exported.Should().Contain("Example Records");

            //var importer = Importer.CreateImporter<TopcatMapping>(Db);
            //importer.SkipBadRecords = true; // see log for skipped bad records
            //importer.Import(@"C:\work\Offshore_survey_TopCat_data_MN.csv");

//            var errors = importer.Results
//                .Where(r => !r.Success)
//                .Select(r => r.Record.Gemini.Title + Environment.NewLine + JsonConvert.SerializeObject(r.Validation) + Environment.NewLine);



        }

        Record ExampleRecord()
        {
            return new Record
            {
                Path = @"x:\some\place",
                Gemini = Library.Example().With(m =>
                {
                    m.ResourceType = "dataset";
                    m.Keywords.Add(new MetadataKeyword { Vocab = "http://vocab.jncc.gov.uk/jncc-domain", Value = "Terrestrial" });
                    m.Keywords.Add(new MetadataKeyword { Vocab = "http://vocab.jncc.gov.uk/jncc-category", Value = "Example Records" });
                }),
            };            
        }

    }
}
