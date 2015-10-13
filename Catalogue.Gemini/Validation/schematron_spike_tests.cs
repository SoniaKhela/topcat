using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Framework;
using Saxon.Api;

namespace Catalogue.Gemini.Validation
{
    class schematron_spike_tests
    {
        [Test, Explicit]
        public void spike()
        {
            var input = new Uri(@"file:\\" + @"\simple\input.sch");
            var xsl = new Uri(@"C:\Users\petmon\Downloads\iso-schematron-xslt2\iso_svrl_for_xslt2.xsl");

            var processor = new Processor();
            var source = processor.NewDocumentBuilder().Build(input);
            var compiler = processor.NewXsltCompiler();
            compiler.ErrorList = new List<Exception>();
            var transformer = compiler.Compile(xsl).Load();

            if (compiler.ErrorList.Count != 0)
                throw new Exception("Exception loading xsl!");

            // Set the root node of the source document to be the initial context node
            transformer.InitialContextNode = source;

            var s = new StringBuilder();
            transformer.Run(new TextWriterDestination(XmlWriter.Create(s)));



            ///////////////////////////////
            // Apply Schemtron xslt 
            ///////////////////////////////
//            var xmlstream = new FileStream(path + @"\simple\input.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
//            var results = new XSLTransform().Transform(xmlstream, schematrontransform);
//
//            System.Diagnostics.Debug.WriteLine("RESULTS");
//            results.Position = 0;
//            System.IO.StreamReader rd2 = new System.IO.StreamReader(results);
//            string xsltSchematronResult = rd2.ReadToEnd();
//            System.Diagnostics.Debug.WriteLine(xsltSchematronResult);
        }

        string GetTestInput()
        {
            return @"
                <?xml version=""1.0"" encoding=""utf-8""?>
                <iso:schema
                  xmlns=""http://purl.oclc.org/dsdl/schematron"" 
                  xmlns:iso=""http://purl.oclc.org/dsdl/schematron""
                  xmlns:dp=""http://www.dpawson.co.uk/ns#""
                  queryBinding='xslt2'
                  schemaVersion='ISO19757-3'>

                  <iso:title>Test ISO schematron file. Introduction mode</iso:title>
                  <iso:ns prefix='dp' uri='http://www.dpawson.co.uk/ns#'/> 

                  <iso:pattern>
                    <iso:rule context=""chapter"">

                      <iso:assert
                         test=""title"">A chapter should have a title</iso:assert>  
                    </iso:rule>
                  </iso:pattern>
                </iso:schema>
                ";
        }

    }
}
