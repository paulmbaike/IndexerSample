using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndexerSample.Entities
{
    [Table("inverted_indices")]
    public class InvertedIndex
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("token")]
        public string Token { get; set; }
        [Column("positions")]
        public long[] Positions { get; set; }
        [Column("doc_guid",TypeName ="uuid")]
        public Guid DocGuid { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        public virtual Document Document {get; set; }
    }
}
