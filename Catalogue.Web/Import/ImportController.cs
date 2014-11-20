using System;
using System.IO;
using System.Web.Http;
using Catalogue.Data.Import;
using Catalogue.Data.Import.Mappings;
using Catalogue.Data.Repository;
using Catalogue.Data.Seed;
using Raven.Client;

namespace Catalogue.Web.Import
{
    /*Slightly weird controller, should only be used once in live system, when importing the data*/
    public class ImportController : ApiController
    {

        private readonly IStore store;

        public ImportController(IStore store)
        {
            this.store = store;
        }

        public class FileSpec
        {
            public string Path { get; set; }         
        }

        public Boolean Post(FileSpec file)
        {

            var importer = Importer.CreateImporter<ActivitiesMapping>(store);
            //importer.SkipBadRecords = true; // todo remove this
            importer.Import(file.Path);

            return true;
        }

        /*using put as is convenient*/
        public Boolean Put()
        {
            Seeder.Seed(store);
            return true;
        }
        
        /*used for testing excpetion handling*/
        public void Get()
        {
            IDocumentStore documentStore = WebApiApplication.DocumentStore;
            
            throw new IOException("My Test Message");
        
        }
    }
}
