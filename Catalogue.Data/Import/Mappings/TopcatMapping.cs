﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalogue.Data.Export;
using Catalogue.Data.Seed;
using Catalogue.Gemini.Model;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

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
            ApplyStandardTopcatConfiguration(config, "\t");
            // do nothing else! for this import mapping we're going to use defaults
        }

        public static CsvConfiguration ApplyStandardTopcatConfiguration(CsvConfiguration config, string delimiter)
        {
            config.Delimiter = delimiter;
            config.PrefixReferenceHeaders = true;
            TypeConverterFactory.AddConverter<List<MetadataKeyword>>(new Exporter.MetadataKeywordConverter());
            TypeConverterFactory.AddConverter<List<Extent>>(new Exporter.ExtentListConverter());

            return config;
        }
    }
}
