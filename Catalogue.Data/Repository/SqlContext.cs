using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalogue.Data.Model;

namespace Catalogue.Data.Repository
{


    public class SqlContext : DbContext
    {
        public SqlContext()
            : base("TopcatDb")
        {
        }

        public DbSet<Record> Records { get; set; }
        public DbSet<Metadata> Metadata { get; set; }
        public DbSet<Extent> Extents { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<Vocabulary> Vocabularies { get; set; }
        public DbSet<BoundingBox> BoundingBoxs { get; set; }
        public DbSet<TemporalExtent> TemporalExtents { get; set; }
        public DbSet<ResponsibleParty> ResponsibleParties { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Metadata>()
                        .ToTable("Metadata");
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

            modelBuilder.Entity<Vocabulary>()
                        .HasMany(v => v.Keywords)
                        .WithOptional(k => k.Vocab)
                        .HasForeignKey(k => k.VocabId);

            modelBuilder.Entity<Keyword>()
                        .HasOptional(k => k.Vocab)
                        .WithMany(v => v.Keywords)
                        .HasForeignKey(k => k.VocabId);



        } 

    }

}
