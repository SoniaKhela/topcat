using System.Collections.Generic;
using System.IO;
using Catalogue.Data.Import.Mappings;
using Catalogue.Data.Model;
using CsvHelper;

namespace Catalogue.Data.Export
{
    public class Exporter
    {
        readonly CsvWriter csv;

        public Exporter(TextWriter writer)
        {
            csv = new CsvWriter(writer);
            TopcatMapping.ApplyStandardTopcatCsvConfiguration(csv.Configuration);
        }

        public void Export(IEnumerable<Record> records)
        {
            csv.WriteRecords(records);
        }

        public void ExportHeader()
        {
            csv.WriteHeader<Record>();
        }

        public void ExportRecord(Record record)
        {
            csv.WriteRecord(record);
        }
    }
}
