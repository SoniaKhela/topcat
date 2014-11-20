using System.Collections.Generic;
using System.Linq;
using Catalogue.Data.Model;
using Catalogue.Utilities.Collections;

namespace Catalogue.Data.Helpers
{
    public static class Extensions
    {
        public static List<Keyword> ToKeywordList(this StringPairList source)
        {
            return source
                .Select((pair => new Keyword { VocabId = pair.Item1, Value = pair.Item2 }))
                .ToList();
        }

        public static bool IsEqualTo(this List<Keyword> source, List<Keyword> other)
        {
            return source.Count == other.Count &&
                source.Zip(other, (a, b) => new { a, b })
                    .All(x => x.a.VocabId == x.b.VocabId && x.a.Value == x.b.Value);
        }

        public static List<Extent> ToExtentList(this StringPairList source)
        {
            return source
                .Select((pair => new Extent { Authority = pair.Item1, Value = pair.Item2 }))
                .ToList();
        }
    }
}
