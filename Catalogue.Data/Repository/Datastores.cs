using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raven.Client;

namespace Catalogue.Data.Repository
{
    public interface IDatastores
    {
        ISqlContext SqlDb { get; }
        IDocumentStore RavenDb { get; }
    }

    public class Datastores : IDatastores
    {
        public Datastores(ISqlContext sqlDb, IDocumentStore  ravenDb)
        {
            this.SqlDb = sqlDb;
            this.RavenDb = ravenDb;
        }

        public ISqlContext SqlDb { get; private set; }
        public IDocumentStore RavenDb { get; private set; }
    }
}
