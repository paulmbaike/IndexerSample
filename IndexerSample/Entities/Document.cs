using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace IndexerSample.Entities
{
    [Table("documents")]
    public class Document
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("doc_path")]
        public string DocPath { get; set; }
        [Column("doc_guid")]
        public Guid DocGuid { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("update_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
