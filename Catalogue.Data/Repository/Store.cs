using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalogue.Data.Model;
using Raven.Client;
using RefactorThis.GraphDiff;

namespace Catalogue.Data.Repository
{
    public interface  IStore 
    {
        IDocumentSession RavenDb { get; }
        SqlContext SqlDb { get; }
        int SaveChangesToAllStores();
    }

    public class Store : IStore, IDisposable
    {
        public Store(IDocumentSession ravenDb, SqlContext sqlDb)
        {
            RavenDb = ravenDb;
            SqlDb = sqlDb;
        }

        public IDocumentSession RavenDb { get; private set; }
        public SqlContext SqlDb { get; private set; }

        public void StoreRecord(Record record) 
        {
            RavenDb.Store(record);

            SqlDb.UpdateGraph(record, recordMap => recordMap
                .OwnedEntity(r => r.Gemini, geminiMap => geminiMap
                    .OwnedCollection(m => m.Extent)
                    .AssociatedCollection(m => m.Keywords)));            
        }

        public int SaveChangesToAllStores()
        {
            RavenDb.SaveChanges();
            return SqlDb.SaveChanges();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // NOTE: Leave out the finalizer altogether if this class doesn't 
        // own unmanaged resources itself, but leave the other methods
        // exactly as they are. 
        ~Store()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources. The bulk of the clean-up code is implemented in Dispose(bool)
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (RavenDb != null) RavenDb.Dispose();
                if (SqlDb != null) SqlDb.Dispose();

            }
        }
    }
}
