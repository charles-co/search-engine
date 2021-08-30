using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Engine {
    public class DbDocument : BaseDocument {
        public readonly long Position;
        private static readonly Queue<BaseDocument> ToBeIndexed = new Queue<BaseDocument>();

        private static readonly Semaphore SaveDocSemaphore = new Semaphore(1, 1);
        private DbDocument(long position, string documentId, string url, string name) : base(name, url, documentId) {
            Position = position;
        }

        public static void IndexDocument(string name, string url) {
            ToBeIndexed.Enqueue(new BaseDocument(name, url));
            SaveToDb();
        }

        private static async void SaveToDb() {
            SaveDocSemaphore.WaitOne();
            var currentDocument = ToBeIndexed.Dequeue();
            if (currentDocument == null) return;
            var name = currentDocument.Name;
            var url = currentDocument.Url;

            var collection = Connector.GetDocumentsCollection();
            var documentIndex = await collection.CountDocumentsAsync(new BsonDocument());
            var bsonDocument = new BsonDocument {
                {"name", name},
                {"url", url},
                {"position", documentIndex}
            };

            await collection.InsertOneAsync(bsonDocument);

            var getDocFilter = Builders<BsonDocument>.Filter.Eq("name", name);
            var createdDoc = collection.Find(getDocFilter).ToList().Last();

            Console.WriteLine($"Document {name} saved to database");

            SaveDocSemaphore.Release();

            Indexer.TryIndex(new DbDocument(documentIndex, createdDoc["_id"].ToString(), url, createdDoc["name"].ToString()));
        }
    }
}