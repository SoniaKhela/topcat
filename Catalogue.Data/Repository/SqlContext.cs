using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalogue.Data.Model;
using Catalogue.Gemini.Model;

namespace Catalogue.Data.Repository
{
    public interface ISqlContext
    {
        DbSet<Record> Records { get; set; }
        DbSet<Metadata> Metadata { get; set; }
        DbSet<Extent> Extents { get; set; }
        DbSet<MetadataKeyword> MetadataKeywords { get; set; }

        int SaveChanges();
    }

    public class SqlContext : DbContext, ISqlContext
    {
        public SqlContext()
            : base("TopcatDb")
        {
        }

        public DbSet<Record> Records { get; set; }
        public DbSet<Metadata> Metadata { get; set; }
        public DbSet<Extent> Extents { get; set; }
        public DbSet<MetadataKeyword> MetadataKeywords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Metadata>()
                        .ToTable("Metadata");
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

        } 

    }

}
