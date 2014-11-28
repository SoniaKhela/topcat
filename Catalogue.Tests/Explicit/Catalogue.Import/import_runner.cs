using Catalogue.Data.Import;
using Catalogue.Data.Import.Mappings;
using Catalogue.Data.Repository;
using Catalogue.Data.Write;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Document;

namespace Catalogue.Tests.Explicit.Catalogue.Import
{
    internal class import_runner
    {
        [Explicit]
        [Test]
        public void run()
        {
            var documentStore = new DocumentStore();
            documentStore.ParseConnectionString("Url=http://localhost:8888/");
            documentStore.Initialize();

            using (var store = new Store(documentStore.OpenSession(), new SqlContext()))
            {
                var vocabService = new VocabularyService(new VocabularyValidator(store), store);
                var importer = new Importer<ActivitiesMapping>(new FileSystem(),
                    new RecordService(new RecordValidator(vocabService),vocabService,store), store);
                importer.Import(@"C:\Work\pressures-data\Human_Activities_Metadata_Catalogue.csv");
            }
        }
    }
}