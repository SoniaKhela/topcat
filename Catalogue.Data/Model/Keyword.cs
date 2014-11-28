using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Raven.Imports.Newtonsoft.Json;

namespace Catalogue.Data.Model
{
    public class Keyword : IComparable<Keyword>
    {
        public Keyword()
        {
        }

        public Keyword(string value, string vocab)
        {
            Value = value;
            VocabId = vocab;
        }

        protected bool Equals(Keyword other)
        {
            return string.Equals(Value, other.Value, StringComparison.InvariantCultureIgnoreCase) && string.Equals(VocabId, other.VocabId, StringComparison.InvariantCultureIgnoreCase);
        }

        public int CompareTo(Keyword other)
        {
            return System.String.Compare(this.Value, other.Value, System.StringComparison.Ordinal);
        }

        public override bool Equals(object obj)
        {
            
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Keyword) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (this.VocabId + "::" + this.Value).GetHashCode()*397;
            }
        }

        [JsonIgnore]
        public int Id { get; set; }

        [JsonIgnore]
        public virtual Vocabulary Vocab { get; set; }

        [JsonIgnore]
        public virtual List<Metadata> Metadata { get; set; }

        [MaxLength(450)]
        public string Value { get; set; }

        [MaxLength(450)]
        public string VocabId { get; set; }
    }
}