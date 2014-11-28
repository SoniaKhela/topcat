using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
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
            //Ensure the metadata table is not pluralised
            modelBuilder.Entity<Metadata>()
                        .ToTable("Metadata");

            //Ensure datetime properties are stored as datetime2 in sql
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

            //Define a one to many relationship between vocabularies in which the keyword also has the id as a property
            modelBuilder.Entity<Vocabulary>()
                        .HasMany(v => v.Keywords)
                        .WithOptional(k => k.Vocab)
                        .HasForeignKey(k => k.VocabId);

            modelBuilder.Entity<Keyword>()
                        .HasOptional(k => k.Vocab)
                        .WithMany(v => v.Keywords)
                        .HasForeignKey(k => k.VocabId);

            //Unique constraint on keyword
            //  Keyword does not need to be in vocab
            modelBuilder
                .Entity<Keyword>()
                .Property(k => k.VocabId)
                .IsOptional()
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_UniqueKeyword", 1) {IsUnique = true}));


            //  But keyword must have a value
            modelBuilder
                .Entity<Keyword>()
                .Property(k => k.Value)
                .IsRequired()
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_UniqueKeyword", 2) {IsUnique = true}));

        }
    }
}
