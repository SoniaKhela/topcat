using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Catalogue.Gemini.Encoding;
using Catalogue.Gemini.Templates;
using Catalogue.Utilities.Clone;
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
            // https://github.com/datagovuk/ckanext-spatial/blob/master/ckanext/spatial/validation/xml/gemini2/Gemini2_R1r3.sch            

            string schematronTransform = @"C:\Work\catalogue\Catalogue.Gemini\Validation\iso_svrl_for_xslt2.xsl";
            string geminiRules = @"C:\Work\catalogue\Catalogue.Gemini\Validation\Gemini2_R1r3.sch.txt";

            // (1) transform the data.gov.uk gemini rules doc with the schematron transform
            string validator = XsltTransform(geminiRules, schematronTransform);
            string validatorPath = @"c:\temp\validator.xslt";
            var sanity = XDocument.Parse(validator);
            sanity.Save(validatorPath);

            // (2) transform the input document with this validator
            var input = new XmlEncoder().Create(Guid.NewGuid(), Library.Example().With(x => x.ResourceType = "blah"));
            string inputPath = @"c:\temp\input.xml";
            input.Save(inputPath);

            string report = XsltTransform(inputPath, validatorPath);

//            var processor = new Processor(); // can be a singleton
//            var compiler = processor.NewXsltCompiler();
//
//            var executable = compiler.Compile(new StringReader(validator));
//            var destination = new DomDestination();
//            using (var inputStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(input)))
//            {
//                var transformer = executable.Load();
//                transformer.SetInputStream(inputStream, new Uri("file:///xxxx"));
//                transformer.Run(destination);
//            }
//
//            var destinationWriter = new StringWriter();
//            destination.XmlDocument.Save(destinationWriter);
//            string results = destinationWriter.ToString();

        }

        string XsltTransform(string inputPath, string xsltPath)
        {
            var processor = new Processor(); // can be a singleton
            var compiler = processor.NewXsltCompiler();

            var executable = compiler.Compile(new Uri(xsltPath));
            var destination = new DomDestination();
            using (var inputStream = File.OpenRead(inputPath))
            {
                var transformer = executable.Load();
                transformer.SetInputStream(inputStream, new Uri("file:///xxx"));
                transformer.Run(destination);
            }

            var destinationWriter = new StringWriter();
            destination.XmlDocument.Save(destinationWriter);
            return destinationWriter.ToString();
        }
    }
}
