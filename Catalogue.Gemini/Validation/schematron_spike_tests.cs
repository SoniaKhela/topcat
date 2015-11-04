using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Catalogue.Gemini.Encoding;
using Catalogue.Gemini.Templates;
using Catalogue.Utilities.Resources;
using FluentAssertions;
using NUnit.Framework;
using Saxon.Api;

namespace Catalogue.Gemini.Validation
{
    class schematron_spike_tests
    {
        [Test, Explicit]
        public void spike2()
        {
            // (1) transform the data.gov.uk gemini rules doc with the schematron transform

            // https://github.com/datagovuk/ckanext-spatial/blob/master/ckanext/spatial/validation/xml/gemini2/Gemini2_R1r3.sch            
//            var schematronTransform = ManifestResource.Open(assembly, "Validation.iso_svrl_for_xslt2.xsl");
//            var geminiRules = ManifestResource.Open(assembly, "Validation.Gemini2_R1r3.sch.txt");

            string schematronTransform = @"C:\Work\catalogue\Catalogue.Gemini\Validation\iso_svrl_for_xslt2.xsl";
            string geminiRules = @"C:\Work\catalogue\Catalogue.Gemini\Validation\Gemini2_R1r3.sch.txt";

            var processor = new Processor(); // can be a singleton
            var compiler = processor.NewXsltCompiler();

            var executable = compiler.Compile(new Uri(schematronTransform));
            var destination = new DomDestination();
            using (var inputStream = File.OpenRead(geminiRules))
            {
                var transformer = executable.Load();
                transformer.SetInputStream(inputStream, new Uri("file:///not/used"));
                transformer.Run(destination);
            }

            var destinationWriter = new StringWriter();
            destination.XmlDocument.Save(destinationWriter);
            string wow = destinationWriter.ToString();

            // (2) transform the input document with this

            var input = new XmlEncoder().Create(Guid.NewGuid(), Library.Example()).ToString();

            var executable2 = compiler.Compile(new StringReader(destinationWriter.ToString()));
            var destination2 = new DomDestination();
            using (var inputStream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(input)))
            {
                
            }
        }

    }
}
