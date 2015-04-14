using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Catalogue.Data.Export;
using Catalogue.Data.Model;
using Catalogue.Utilities.Clone;
using Raven.Abstractions.Extensions;
using Raven.Client;
using Raven.Client.Document;
using Raven.Database.FileSystem.Extensions;
using Raven.Database.Indexing.Collation;

namespace Catalogue.Web.Controllers.Export
{
    public class ExportController : ApiController
    {

        public async Task<HttpResponseMessage> Get([FromUri] RecordQueryInputModel input)
        {

            return await new TaskFactory().StartNew(
               () =>
               {
                   var memStream = new MemoryStream();
                   var streamWriter = new StreamWriter(memStream);
                   
                   var db = WebApiApplication.DocumentStore.OpenSession();
                   var queryer = new RecordQueryer(db);

                   new Exporter().Export(FetchRecords(db,queryer, input), streamWriter);

                   streamWriter.Flush();

                   var result = Request.CreateResponse(HttpStatusCode.OK);
                   result.Content = new StreamContent(memStream);


                   result.Headers.AcceptRanges.Add("bytes");
                   result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                   {
                       FileName = "topcat-export-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt"
                   };
                   result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                   result.Content.Headers.ContentLength = memStream.Length;
                   
                   return result;
               });
        }


         //<summary>
         //This is public because I don't have time right now to split things up to test it any better.
         //</summary>
        public static IEnumerable<Record> FetchRecords(IDocumentSession db, RecordQueryer queryer, RecordQueryInputModel input)
        {
                      
            var query = input.With(x =>
                {
                    x.P = 0;
                });


            using (var e = db.Advanced.Stream(queryer.RecordQuery(query)))
            {
                while (e.MoveNext())
                {
                    yield return e.Current.Document;
                }
            }
        }
    }
}
