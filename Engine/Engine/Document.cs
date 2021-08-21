using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver;

namespace Engine {
    public class Document {
        public long position;
        public string documentId;
        public string url;

        public Document(long position, string documentId, string url) {
            this.position = position;
            this.documentId = documentId;
            this.url = url;
        }

        public static async void IndexDocument(string name, string url) {
            Console.WriteLine($"Creating new document: {name}");
            var collection = Connector.GetDocumentsCollection();
            var documentIndex = await collection.CountDocumentsAsync(new BsonDocument());
            
            var bsonDocument = new BsonDocument {
                { "name", name },
                { "url", url },
                { "position", documentIndex }
            };

            await collection.InsertOneAsync(bsonDocument);
            
            var getDocFilter = Builders<BsonDocument>.Filter.Eq("name", name);
            var createdDoc = collection.Find(getDocFilter).ToList().Last();

            Console.WriteLine($"Document {name} saved to database");
            
            Indexer.TryIndex(new Document(documentIndex, createdDoc["_id"].ToString(), url));
        }
    }
}