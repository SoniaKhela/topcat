﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalogue.Data.Model;
using Catalogue.Tests.Utility;
using FluentAssertions;
using NUnit.Framework;

namespace Catalogue.Tests.Slow.Catalogue.Data.Indexes
{
    class records_search_index_specs : DatabaseTestFixture
    {
        [Test]
        public void can_search_partial_matches()
        {
            var results = Db.Advanced.LuceneQuery<Record>("Records/Search")
                .Search("TitleN", "stu") // search the ngrammed title field for 'stu'
                .Take(100)
                .ToList();

            results.Count.Should().Be(13);
            results.Any(r => r.Gemini.Title.Contains("Study")).Should().BeTrue();
            results.Any(r => r.Gemini.Title.Contains("Estuaries")).Should().BeTrue();
        }
    }
}
