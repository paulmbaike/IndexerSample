using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IndexerSample.Models
{
     public interface ITokenDocument
    {
        string Term { get; }
        Dictionary<string, HashSet<long>> Docs { get; }
        void removeDocument(string docId);
        void addDocument(string docId, HashSet<long> positions);

    }
    public class TokenDocument : ITokenDocument
    {   
        [BsonId]
        public string Term
        {   get{ return Term; }
            set {
                Term = value.ToLower();
            }
        }

        [BsonElement("docs")]
        public Dictionary<string, HashSet<long>> Docs { get; }

        [BsonConstructor]
        public TokenDocument(string term){
            this.Term = term;
            this.Docs = new Dictionary<string, HashSet<long>>();
        }

        public void addDocument(string docId, HashSet<long> positions) {
            Docs.Add(docId, positions);
        }

        public void removeDocument(string docId) {
            Docs.Remove(docId);
        }
    }
}