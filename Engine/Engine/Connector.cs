using MongoDB.Bson;
using MongoDB.Driver;

namespace Engine {
    public class Connector {
        private static MongoClient Client;

        private static string database = "404Db";
        private static string mongoURL =
            $"mongodb://127.0.0.1:27017/{database}?retryWrites=true&w=majority";
        
        public static void GenerateDb() {
            Client = new MongoClient(mongoURL);
        }

        public static void SetTestMode()
        {
            database = "404Db_Test";
            mongoURL = $"mongodb://127.0.0.1:27017/{database}?retryWrites=true&w=majority";
        }

        private static IMongoDatabase GetDb() {
            if (Client == null) {
                Client = new MongoClient(mongoURL);
            }

            return Client.GetDatabase(database);
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
        
        public static IMongoCollection<BsonDocument> GetSavedQueriesCollection() {
            var db = GetDb();
            return db.GetCollection<BsonDocument>("savedQueries");
        }
    }
}