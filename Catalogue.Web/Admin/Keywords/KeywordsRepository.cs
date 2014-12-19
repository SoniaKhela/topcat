using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using Catalogue.Data.Indexes;
using Catalogue.Data.Model;
using Catalogue.Data.Repository;
using Raven.Client;

namespace Catalogue.Web.Admin.Keywords
{
    public interface IKeywordsRepository
    {
        Keyword Create(String value, String vocab);
        void Delete(Keyword keyword);
        ICollection<Keyword> Read(String value, String vocab);
        ICollection<Keyword> ReadByVocab(string vocab);
        ICollection<Keyword> ReadByValue(string value);
        ICollection<Keyword> ReadAll();
    }

    public class KeywordsRepository : IKeywordsRepository
    {
        private readonly IStore _db;


        public KeywordsRepository(IStore db)
        {
            _db = db;
        }

        public Keyword Create(string value, string vocab)
        {
            throw new NotImplementedException();
        }

        public void Delete(Keyword keyword)
        {
            throw new NotImplementedException();
        }

        public ICollection<Keyword> Read(string value = null, string vocab = null)
        {
            throw new NotImplementedException();
        }

        public ICollection<Keyword> ReadByVocab(string vocab)
        {
            throw new NotImplementedException();
        }

        public ICollection<Keyword> ReadByValue(string value)
        {
            if (value == null) return new List<Keyword>();

            return GetKeywordByValue(value);
        }

        public ICollection<Keyword> ReadAll()
        {
            return GetKeywordByValue(null);
        }

        private ICollection<Keyword> GetKeywordByValue(string value)
        {
            return (from k in _db.SqlDb.Keywords
                    where value == null || k.Value.Contains(value)
                    select k).ToList();
        }

    }
}