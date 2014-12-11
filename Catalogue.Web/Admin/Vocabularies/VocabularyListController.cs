using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Catalogue.Data.Indexes;
using Catalogue.Data.Repository;
using Raven.Client;

namespace Catalogue.Web.Controllers.Vocabularies
{
    public class VocabularyListResult
    {
        public string Vocab { get; set; }
        public string Description { get; set; }
    }

    public class VocabularyListController : ApiController
    {
        private readonly IStore _db; 

        public VocabularyListController(IStore db)
        {
            _db = db;
        }

        public ICollection<VocabularyListResult> Get()
        {
            return (from v in _db.SqlDb.Vocabularies
                    orderby v.Id descending 
                    select new VocabularyListResult()
                        {
                            Vocab = v.Id,
                            Description = v.Description
                        }).ToList();
        }
    }
}