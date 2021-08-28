using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IndexerSample.Models;
using IndexerSample.Components;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace IndexerSample.Services.Helpers
{

    public interface IUtil {
        Task<string> GetDocumentID(string path);
        Task<List<ITokenDocument>> GetTokenDocuments();
        Task<List<ITokenDocument>> GetTokenDocumentsByTerms(List<string> terms);
        Task<string> AddDocument(String path);
        Task AddTokenDocument(string term, string docId, HashSet<long> positions);
        Task<bool?> RemoveDocument(string id);
        Task ClearDeletedDocuments(string id);
        Task<HashSet<IDocument>> GetDocuments();
        Task<HashSet<IDocument>> GetDocumentsByID(List<string> docIds);
        Task<IDocument> GetDocument(string id);
    }

    public class Util : IUtil
    {
        public static Util util;
        private IMongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<TokenDocument> _invertedIndicesTable;
        private IMongoCollection<Document> _documentsTable;
        private readonly ILogger<Util> _logger;
        private readonly Indexer _indexer;

        static Util(){
            if (!BsonClassMap.IsClassMapRegistered(typeof(TokenDocument))) {
                // register class map for TokenDocument and Document
                BsonClassMap.RegisterClassMap<TokenDocument>();
                BsonClassMap.RegisterClassMap<Document>();
            }
        }

        public Util(ILogger<Util> logger, IMongoClient client, IMongoDatabase database)
        {
            if (client == null || database == null)
            {
                throw new ArgumentNullException("MongoDB Client and Database are required");
            }
            _client = client;
            _database = database;
            _logger = logger;
            _indexer = new Indexer(util);
            //_indexer.IndexRespository();
            initDB();
            _invertedIndicesTable = database.GetCollection<TokenDocument>("InvertedIndicesTable");
            _documentsTable = database.GetCollection<Document>("DocumentsTable");
        }

        public static void init()
        {
            IMongoClient _client;
            IMongoDatabase _database;
            _client = new MongoClient();
            _database = _client.GetDatabase("indexersample");
            ILoggerFactory factory = new LoggerFactory();
            ILogger<Util> log = new Logger<Util>(factory);
            util = new Util(log, _client, _database);
        }

        public async void initDB() {
            if(!await CheckIfCollectionExists("InvertedIndicesTable")){
                _database.CreateCollection("InvertedIndicesTable");
            }
            if(!await CheckIfCollectionExists("DocumentsTable")){
                _database.CreateCollection("DocumentsTable");
            }
        }

        private async Task<bool> CheckIfCollectionExists(string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            var collectionCursor = await _database.ListCollectionsAsync(new ListCollectionsOptions {Filter = filter});
            return await collectionCursor.AnyAsync();
        }
        public async Task<IDocument> GetDocument(string id)
        {
            var filter = Builders<Document>.Filter.Eq("_id", id);
            Document result = await _documentsTable.Find(filter).FirstOrDefaultAsync();
            return result;
        }
        public async Task<HashSet<IDocument>> GetDocuments()
        {
            HashSet<IDocument> result = new HashSet<IDocument>();
            await (await _documentsTable.FindAsync(FilterDefinition<Document>.Empty)).ForEachAsync(t => result.Add(t));
            if (result.Count < 1)
            {
                return new HashSet<IDocument>();
            }
            return result;
        }
        public async Task<HashSet<IDocument>> GetDocumentsByID(List<string> docIds)
        {
            var filter = Builders<Document>.Filter.AnyIn("_id", docIds);
            HashSet<IDocument> result = new HashSet<IDocument>();
            await (await _documentsTable.FindAsync(filter)).ForEachAsync(t => result.Add(t));
            if (result.Count < 1)
            {
                return new HashSet<IDocument>();
            }
            return result;
        }
        public async Task<List<ITokenDocument>> GetTokenDocuments()
        {
            List<ITokenDocument> result = new List<ITokenDocument>();
            await (await _invertedIndicesTable.FindAsync(FilterDefinition<TokenDocument>.Empty)).ForEachAsync(t => result.Add(t));
            return result;
        }
        public async Task<List<ITokenDocument>> GetTokenDocumentsByTerms(List<string> terms)
        {
            var filter = Builders<TokenDocument>.Filter.AnyIn("term", terms);
            List<ITokenDocument> result = new List<ITokenDocument>();
            await (await _invertedIndicesTable.FindAsync(filter)).ForEachAsync(t => result.Add(t));
            return result;
        }
        public async Task<string> GetDocumentID(string path)
        {
            var filter = Builders<Document>.Filter.Eq("path", path);
            Document result = await _documentsTable.Find(filter).FirstOrDefaultAsync();
            if (result == null) return null;
            return result["_id"].ToString();
        }

        public async Task<string> AddDocument(string path)
        {
            await _documentsTable.InsertOneAsync(new Document(path));
            string id = await GetDocumentID(path);
            return id;
        }

        public async Task AddTokenDocument(string term, string docId, HashSet<long> positions)
        {
            TokenDocument newDoc = new TokenDocument(term);
            newDoc.addDocument(docId, positions);
            await _invertedIndicesTable.InsertOneAsync(newDoc);
        }
        public async Task<bool?> RemoveDocument(string id)
        {
            //Remove document from document table
            var filter = Builders<Document>.Filter.Eq("_id", new ObjectId(id));
            var result = await _documentsTable.DeleteOneAsync(filter);
            if (!result.IsAcknowledged) return false;

            //Clear terms with empty documents from database
            var clearFilter = Builders<BsonDocument>.Filter.SizeLte("docs", 0);
            await _invertedIndicesTable.DeleteManyAsync(t => t.Docs.Count < 1);

            await ClearDeletedDocuments(id);
            return true;
        }

        public async Task ClearDeletedDocuments(string id) {
            //Delete the document from all occurences in the token documents table 
            var bulkDeleteUpdateFilter = Builders<TokenDocument>.Update.PullFilter("docs", Builders<BsonDocument>.Filter.Eq("doc_id", new ObjectId(id)));
            await _invertedIndicesTable.UpdateManyAsync(Builders<TokenDocument>.Filter.Eq("docs.doc_id", new ObjectId(id)), bulkDeleteUpdateFilter);
        }
    }
}
