using System;
using IndexerSample.Entities;
using Microsoft.EntityFrameworkCore;

namespace IndexerSample.Helpers
{
    public class DatabaseContext: DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<InvertedIndex> InvertedIndices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder.Entity<InvertedIndex>()
            //.HasOne(p => p.Document)
            //.WithMany()
            //.HasForeignKey(p => p.DocId);

        }
    }
}
