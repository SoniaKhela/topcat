using System;
using System.Collections.Generic;
using System.Linq;
using Catalogue.Data.Model;
using Catalogue.Data.Repository;
using Raven.Client;

namespace Catalogue.Data.Write
{
    public interface IVocabularyValidator
    {
        VocabularyValidationResult Valdiate(Vocabulary sourceVocab, bool allowControlledUpdates);
    }

    public class VocabularyValidationResult
    {
        public List<VocabularyValidationResultMessage> Errors { get; set; }
        public List<VocabularyValidationResultMessage> Warnings { get; set; }
    }

    public class VocabularyValidationResultMessage
    {
        public string Message { get; set; }
        public string FieldName { get; set; }
    }

    public class VocabularyValidator : IVocabularyValidator
    {
        //check uri format is correct
        //check publication date format.
        //check existing keyword values haven't been changed to duplicate other existing values

        private readonly IStore store;

        public VocabularyValidator(IStore store)
        {
            this.store = store;
        }

        public VocabularyValidationResult Valdiate(Vocabulary sourceVocab, bool allowControlledUpdates)
        {
            var result = new VocabularyValidationResult {Errors = new List<VocabularyValidationResultMessage>(), Warnings = new List<VocabularyValidationResultMessage>()};
            //check uri format is correct
            var r1 = ValidateVocabularyUri(sourceVocab.Id);
            if (r1 != null) result.Errors.Add(r1);

            var targetVocab = store.SqlDb.Vocabularies.SingleOrDefault(x => x.Id == sourceVocab.Id);

            //check existing keyword values haven't been changed to duplicate other existing values
            result.Errors.AddRange(ValidateKeywordChanges(sourceVocab, targetVocab));
            
            //Check for updates to controlled vocabs and duplicates that will be removed and warn
            result.Warnings.AddRange(ValidateKeywordAdditions(sourceVocab, targetVocab));

            //validate additions to controlled vocabs

            result.Errors.AddRange(ValidateControlledVocab(sourceVocab, targetVocab, allowControlledUpdates));

            //check publication date format.
            var r2 = ValidatePublicationDate(sourceVocab.PublicationDate);
            if (r2 != null) result.Errors.Add(r2);

            return result;
        }

        private List<VocabularyValidationResultMessage> ValidateControlledVocab(Vocabulary sourceVocab, Vocabulary targetVocab, bool allowControlledUpdates)
        {
            var result = new List<VocabularyValidationResultMessage>();

            if (targetVocab == null || !targetVocab.Controlled || allowControlledUpdates) return result;

            IEnumerable<Keyword> newKeywords;
            newKeywords = sourceVocab.Keywords.Where(x => x.Id == 0);

            foreach (var newKeyword in newKeywords)
            {
                result.Add(new VocabularyValidationResultMessage
                    {
                        FieldName = newKeyword.Vocab,
                        Message =
                            String.Format(
                                "The keyword {0} cannot be added to the vocabulary {1} because it is controlled",
                                newKeyword.Value, targetVocab.Id)
                    });
            }

            return result;
        }

        private IEnumerable<VocabularyValidationResultMessage> ValidateKeywordAdditions(Vocabulary source, Vocabulary target)
        {
            if (target == null) return new List<VocabularyValidationResultMessage>();

            //Any new keywords that duplicate existing ones
            var duplicates =
                (from dk in
                     target.Keywords.Where(
                         t =>
                         source.Keywords.Where(k => k.Id == 0)
                                    .Select(k => k.Value.ToLower())
                                    .Contains(t.Value.ToLower()))
                 select new VocabularyValidationResultMessage
                     {
                         FieldName = dk.Id.ToString(),
                         Message =
                             String.Format("The keyword {0} is duplicated and the duplicate will not be saved.",
                                           dk.Value)
                     }).ToList();

            return duplicates;
        }

        private IEnumerable<VocabularyValidationResultMessage> ValidateKeywordChanges(Vocabulary source, Vocabulary target)
        {
            var results = new List<VocabularyValidationResultMessage>();

            if (target != null)
            {
                results.AddRange(from sk in source.Keywords.Where(sk => sk.Id != 0 && target.Keywords.Any(tk => tk.Id != sk.Id && tk.Value.ToLower() == sk.Value.ToLower()))
                                 let sk1 = sk
                                 let tk = target.Keywords.Single(k => k.Id == sk1.Id)
                                 where tk != null
                                 select new VocabularyValidationResultMessage
                                     {
                                         FieldName = sk.Id.ToString(), Message = String.Format("Cannot change the value of keyword {0} to {1} because it will create a duplicate keyword", tk.Value, sk.Value)
                                     });
            }


            return results;
            //select from source where source value doesn't match target value

        }

        private VocabularyValidationResultMessage ValidatePublicationDate(string publicationDate)
        {
            if (String.IsNullOrWhiteSpace(publicationDate)) return null;
            
            DateTime date;
            
            if (!DateTime.TryParse(publicationDate, out date))
            {
                return new VocabularyValidationResultMessage
                    {
                        FieldName = "PublicationDate",
                        Message = String.Format("{0} cannot be parsed as a valid date", publicationDate)
                    };
            }

            return null;
        }

        private VocabularyValidationResultMessage ValidateVocabularyUri(string id)
        {
            Uri url;


            if (String.IsNullOrWhiteSpace(id))
            {
                return new VocabularyValidationResultMessage
                    {
                        Message = "A vocabulary must have a properly formed Id",
                        FieldName = "Id"
                    };
            }

            if (Uri.TryCreate(id, UriKind.Absolute, out url))
            {
                if (url.Scheme != Uri.UriSchemeHttp)
                {

                    return new VocabularyValidationResultMessage
                        {
                            Message = String.Format("Resource locator {0} is not an http url", id),
                            FieldName = "Id"
                        };
                }
            }
            else
            {
                return new VocabularyValidationResultMessage
                    {
                        Message = String.Format("Resource locator {0} is not a valid url", id),
                        FieldName = "Id"
                    };
            }

            return null;
        }
    }
}
