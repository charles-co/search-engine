using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver;

namespace Engine {
    internal class UnsavedDocument {
        public string name;
        public string url;

        public UnsavedDocument(string name, string url) {
            this.name = name;
            this.url = url;
        }
    }
    
    public class Document {
        public long position;
        public string documentId;
        public string url;
        private static Queue<UnsavedDocument> toBeIndexed = new Queue<UnsavedDocument>();

        private static Semaphore _indexDoc = new Semaphore(1, 1);
        private Document(long position, string documentId, string url) {
            this.position = position;
            this.documentId = documentId;
            this.url = url;
        }

        public static void IndexDocument(string name, string url) {
            toBeIndexed.Enqueue(new UnsavedDocument(name, url));
            SaveToDb();
        }

        private static async void SaveToDb() {
            _indexDoc.WaitOne();
            var currentDocument = toBeIndexed.Dequeue();
            var name = currentDocument.name;
            var url = currentDocument.url;
            
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
            
            _indexDoc.Release();
            
            Indexer.TryIndex(new Document(documentIndex, createdDoc["_id"].ToString(), url));

        }
    }
}