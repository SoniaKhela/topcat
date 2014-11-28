using System;
using System.Collections.Generic;
using System.Linq;
using Catalogue.Data.Model;
using Catalogue.Data.Repository;
using Raven.Client;
using RefactorThis.GraphDiff;

namespace Catalogue.Data.Write
{
    public interface IVocabularyService
    {
        Vocabulary Load(string id);
        ICollection<VocabularyServiceResult> UpsertKeywords(List<Keyword> keywords);
        VocabularyServiceResult Insert(Vocabulary vocab);
        VocabularyServiceResult Update(Vocabulary vocab);
    }

    public class VocabularyService : IVocabularyService
    {
        private readonly IStore store;
        private readonly IVocabularyValidator validator;
        

        public VocabularyService(IVocabularyValidator validator, IStore store)
        {
            this.store = store;
            this.validator = validator;
        }

        public Vocabulary Load(string id)
        {
            return store.SqlDb.Vocabularies.FirstOrDefault(v => v.Id == id);
        }

        private VocabularyServiceResult UpsertVocabulary(Vocabulary vocab, VocabularyValidationResult validationResult)
        {

            //remove duplicates of existing keywords
            var existingKeywords = store.SqlDb.Keywords.Where(k => k.VocabId == vocab.Id).Select(v => v.Value);
            vocab.Keywords = vocab.Keywords.Where(k => !existingKeywords.Contains(k.Value)).ToList();
            vocab.Keywords = RemoveDuplicateKeywords(vocab.Keywords).ToList();

            store.SqlDb.UpdateGraph(vocab, map =>
                                           map.OwnedCollection(v => v.Keywords));

            return new VocabularyServiceResult
            {
                Success = true,
                Vocab = vocab,
                Validation = validationResult
            };
        }

        private IEnumerable<Keyword> RemoveDuplicateKeywords(IEnumerable<Keyword> keywords)
        {
            return (from i in keywords
                    group i by i.Value.ToLowerInvariant()
                    into g
                    select g.OrderBy(p => p.Value).First());
        }


        private VocabularyServiceResult AppendKeywords(Vocabulary vocab)
        {
            var validationResult = validator.Valdiate(vocab, allowControlledUpdates:false);

            if (validationResult.Errors.Any())
                return new VocabularyServiceResult
                {
                    Success = false,
                    Vocab = vocab,
                    Validation = validationResult
                };

            var v = store.SqlDb.Vocabularies.SingleOrDefault(x => x.Id == vocab.Id);

            if (v == null)
            {
                //Create new vocab
                var result =  UpsertVocabulary(vocab, validationResult);
                if (result.Success) store.SqlDb.SaveChanges();
                return result;
            }

            var newKeywords = vocab.Keywords.Where(x => !v.Keywords.Select(k => k.Value.ToLower()).Contains(x.Value.ToLower()));
            v.Keywords.AddRange(newKeywords);
            store.SqlDb.SaveChanges();

            return new VocabularyServiceResult
            {
                Success = true,
                Vocab = vocab,
                Validation = validationResult
            };
 
        }

        public ICollection<VocabularyServiceResult> UpsertKeywords(List<Keyword> keywords)
        {
            if (keywords == null) return new List<VocabularyServiceResult>();

            return (from vocabId in keywords.Select(k => k.VocabId)
                    where !String.IsNullOrWhiteSpace(vocabId)
                    select new Vocabulary
                        {
                            Id = vocabId,
                            Controlled = false,
                            Name = vocabId,
                            Description = String.Empty,
                            PublicationDate = DateTime.Now.ToString("MM-yyyy"),
                            Publishable = true,
                            Keywords = 
                                keywords.Where(k => k.VocabId == vocabId)
                                        .Select(k => new Keyword(){Value = k.Value})
                                        .ToList()
                        }
                    into vocab
                    select AppendKeywords(vocab)).ToList();
        }

        public VocabularyServiceResult Insert(Vocabulary vocab)
        {
            var validationResult = validator.Valdiate(vocab, allowControlledUpdates: true);

            if (validationResult.Errors.Any())
            return new VocabularyServiceResult
                {
                    Success = false,
                    Vocab = vocab,
                    Validation = validationResult
                };


            if (store.SqlDb.Vocabularies.Any(x => x.Id == vocab.Id))
            {
                validationResult.Errors.Add(new VocabularyValidationResultMessage
                    {
                        FieldName = "Id",
                        Message = String.Format("A vocabulary with id {0} already exists", vocab.Id)
                    });

                return new VocabularyServiceResult
                {
                    Success = false,
                    Vocab = vocab,
                    Validation = validationResult
                };
            }

            return UpsertVocabulary(vocab, validationResult);

        }

        public VocabularyServiceResult Update(Vocabulary vocab)
        {
            var validationResult = validator.Valdiate(vocab, allowControlledUpdates: true);

            if (validationResult.Errors.Any())
                return new VocabularyServiceResult
                {
                    Success = false,
                    Vocab = vocab,
                    Validation = validationResult
                };

            return UpsertVocabulary(vocab, validationResult);
        }
    }


    public class VocabularyServiceResult
    {
        public Vocabulary Vocab { get; set; }
        public bool Success { get; set; }
        public VocabularyValidationResult  Validation { get; set; }
    }
}
