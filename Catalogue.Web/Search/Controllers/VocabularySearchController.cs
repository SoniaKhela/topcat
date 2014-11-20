﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Catalogue.Web.Controllers.Search;
using Catalogue.Web.Search.Service;

namespace Catalogue.Web.Search.Controllers
{
    public class VocabularySearchController : ApiController
    {
        private readonly ISearchService _vocabSearchService;

        public VocabularySearchController(ISearchService vocabSearchService)
        {
            _vocabSearchService = vocabSearchService;
        }

        public SearchOutputModel Get(string vocab, int n = 25, int p = 0)
        {
            var searchInputModel = new SearchInputModel()
            {
                Query = vocab,
                NumberOfRecords = n,
                PageNumber = p,
                SearchType = SearchType.Vocabulary
            };
            var output = _vocabSearchService.FindByVocab(searchInputModel);
            return output;
        }
    }
}