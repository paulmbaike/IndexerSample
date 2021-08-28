using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IndexerSample.Models
{
    public interface IDocument{
        string Path { get; }

    }
    public class Document : BsonDocument, IDocument {
        [BsonElement("path")]
        public string Path { get; }

        [BsonConstructor]
        public Document(string path){
            this.Path = path;
        }

    }
}