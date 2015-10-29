using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Xml.Linq;
using Catalogue.Data.Export;
using Catalogue.Data.Model;
using Catalogue.Data.Query;
using Catalogue.Gemini.Encoding;
using Catalogue.Utilities.Clone;
using Newtonsoft.Json;
using Raven.Abstractions.Data;
using Raven.Abstractions.Util;
using Raven.Client;

namespace Catalogue.Web.Controllers.Export
{
    public class ExportController : ApiController
    {
        IDocumentSession _db;
        IRecordQueryer recordQueryer;

        public ExportController(IDocumentSession db, IRecordQueryer recordQueryer)
        {
            this._db = db;
            this.recordQueryer = recordQueryer;
        }

        void RemovePagingParametersFromRecordQuery(RecordQueryInputModel input)
        {
            input.P = 0;
            input.N = -1; 
        }

        /// <summary>
        /// Exports a csv file of records using the standard export format. 
        /// Note: ignores the paging parameters P and N.
        /// </summary>
        public HttpResponseMessage Get([FromUri] RecordQueryInputModel input)
        {
            RemovePagingParametersFromRecordQuery(input);
    
            var response = MakeResponse(input, async (enumerator, stream) =>
            {
                var writeHeaders = true;
                while (await enumerator.MoveNextAsync())
                {
                    var writer = new StringWriter();
                    var exporter = new Exporter();
                    if (writeHeaders)
                    {
                        exporter.ExportHeader(writer);
                        writeHeaders = false;
                    }

                    exporter.ExportRecord(enumerator.Current.Document, writer);
                    var data = Encoding.UTF8.GetBytes(writer.ToString());

                    await stream.WriteAsync(data, 0, data.Length);
                }

            });

            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "topcat-export-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".csv"
                };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return response;
        }


        /// <summary>
        /// Exports an ISO XML file of records.
        /// </summary>
        [HttpGet, Route("api/export/xml")]
        public HttpResponseMessage Xml([FromUri] RecordQueryInputModel input)
        {
            RemovePagingParametersFromRecordQuery(input);

            var records = recordQueryer.RecordQuery(input);

            // encode the records as iso xml elements
            var elements = from record in records
                           let doc = new XmlEncoder().Create(record.Id, record.Gemini)
                           select doc.Root;

            var output = new XDocument(new XElement("topcat-export", elements)).ToString();

            var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(output) };
            return result;
        }


        /// <summary>
        /// Template function for doing the untidy async coordination between raven and asp.net streaming.
        /// </summary>
        HttpResponseMessage MakeResponse(RecordQueryInputModel input, Action<IAsyncEnumerator<StreamResult<Record>>, Stream> performExport)
        {
            using (var adb = _db.Advanced.DocumentStore.OpenAsyncSession())
            {
                var response = new HttpResponseMessage();

                response.Content = new PushStreamContent(
                    async (stream,  content,  context) =>
                    {
                        using (stream)
                        using (var enumerator = await adb.Advanced.StreamAsync(recordQueryer.AsyncRecordQuery(adb, input)))
                        {
                            performExport(enumerator, stream);
                        }
                    });

                return response;
            }
        }
    }
}
