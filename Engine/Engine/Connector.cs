using MongoDB.Bson;
using MongoDB.Driver;

namespace Engine {
    public class Connector {
        private static MongoClient Client;
        
        private static IMongoDatabase GetDb() {
            if (Client == null) {
                Client = new MongoClient("mongodb+srv://user:user@querydata.q0xxx.mongodb.net");
            }

            return Client.GetDatabase("404Db");
        }

        //TODO: Make word the index to make retrieval faster
        // private static async void BuildDocumentIndex(IMongoDatabase db) {
        //     var collection = db.GetCollection<BsonDocument>("tokens");
        //     var indexKeysDefinition = Builders<BsonDocument>.IndexKeys.Ascending(token => token.word);
        //     await collection.Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(indexKeysDefinition));
        // }
        
        public static IMongoCollection<BsonDocument> GetDocumentsCollection() {
            var db = GetDb();
            return db.GetCollection<BsonDocument>("documents");
        }
        
        public static IMongoCollection<BsonDocument> GetTokensCollection() {
            var db = GetDb();
            return db.GetCollection<BsonDocument>("tokens");
        }
    }
}