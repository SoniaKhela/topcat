using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Web.Http;
using Catalogue.Data.Repository;
using Raven.Client;
using System.Linq;

namespace Catalogue.Web.Controllers.Search
{
    public class VocabularyTypeaheadController : ApiController
    {
        private readonly IStore _db;

        public VocabularyTypeaheadController(IStore db)
        {
            _db = db;
        }

        public ICollection<string> Get(String q)
        {
            if (String.IsNullOrWhiteSpace(q)) return new List<string>();

            return (from v in _db.SqlDb.Vocabularies
                    where v.Id.Contains(q)
                    select v.Id).ToList();
        }
    }
}