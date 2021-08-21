using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver;

namespace Engine {
    public class Document {
        public int documentId;
        public string url;

        public Document(int documentId, string url) {
            this.documentId = documentId;
            this.url = url;
        }
        
        public static Document CreateDocument(string name, string url) {
            MongoClient dbClient = new MongoClient("mongodb+srv://user:user@querydata.q0xxx.mongodb.net");
            var database = dbClient.GetDatabase("404Db");
            var collection = database.GetCollection<BsonDocument>("documents");

            var document = new BsonDocument {
                { "name", name },
                { "url", url }
            };

            var createdDoc = collection.InsertOneAsync(document);

            var documentObj = new Document(createdDoc.Id, url);

            return documentObj;
        }
    }
}