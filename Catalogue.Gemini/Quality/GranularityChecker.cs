using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using System.IO;

namespace Catalogue.Gemini.Quality
{
    public class GranularityChecker
    {
        static string[] Tokenize(string s)
        {
            return s.Split(',').Select(word => word.Trim().ToLower()).ToArray();
        }

        public static int CountIdenticalPairs(string[] a, string[] b)
        {
            var aPairs = a.Zip(a.Skip(1), (t, u) => new { t, u });
            var bPairs = b.Zip(b.Skip(1), (t, u) => new { t, u });

            return aPairs.Intersect(bPairs).Count();
        }

        //        int HowManyRecordsDoesThisRecordHave5OrMoreSameWordsAs(string[] words, int n, List<TokenizedRecord> tokenized)
        //        {
        //            // for each record, how many identical pairs of words are the in this record?
        //            tokenized.Where(r => r.Title)
        //        }

        public GranularityResult Check(List<RecordStub> input)
        {
            var tokenizedRecords = input.Select(r => new TokenizedRecord { Title = Tokenize(r.Title), Abstract = Tokenize(r.Abstract) }).ToList();

//            foreach (var record in input)
//            {
//                // look at the record and compare it to the others. there should only be one record which has as many matching words as it
//                record.Title
//            }

            var q = from x in input
                    group x by x.Title;

            return new GranularityResult { Groups = q.Select(g => new List<RecordStub>(g)).ToList() };
        }

    }


    public class TokenizedRecord
    {
        public string[] Title { get; set; }
        public string[] Abstract { get; set; }
    }
    public class GranularityResult
    {
        public List<List<RecordStub>> Groups { get; set; }

    }

    public class RecordStub
    {
        public string Title { get; set; }
        public string Abstract { get; set; }
    }

    public class tests
    {
        [Test]
        public void blah()
        {
            //var lines = File.ReadLines(@"C:\work\data\Data Services metadata for top cat entry.csv");

//            var result = GranularityChecker.Check(TestData);
//            result.Groups.Count.Should().Be(2);
        }

        [Test]
        public void can_count_duplicate_pairs()
        {
            var a = new [] { "one", "two", "three", "four" };
            var b = new [] { "naught", "one", "two", "three" };

            GranularityChecker.CountIdenticalPairs(a, b).Should().Be(2);
        }

        List<RecordStub> TestData = new List<RecordStub>
        {
            new RecordStub { Title = "This is a title 1", Abstract = "This is an abstract 1" },
            new RecordStub { Title = "This is a title 2", Abstract = "This is an abstract 2" },
            new RecordStub { Title = "This is a title 3", Abstract = "This is an abstract 3" },
            new RecordStub { Title = "Bom bom bom 1", Abstract = "Boom boom boom 1" },
            new RecordStub { Title = "Bom bom bom 2", Abstract = "Boom boom boom 2" },
            new RecordStub { Title = "Bom bom bom 3", Abstract = "Boom boom boom 3" },
        };
    }
}
