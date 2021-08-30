using MongoDB.Bson;
using MongoDB.Driver;

namespace Engine {
    public static class Connector {
        private static MongoClient _client;

        private static string _database = "404Db";
        private static string _mongoUrl =
            $"mongodb://127.0.0.1:27017/{_database}?retryWrites=true&w=majority";
        
        public static void GenerateDb() {
            _client = new MongoClient(_mongoUrl);
        }

        public static void SetTestMode()
        {
            _database = "404Db_Test";
            _mongoUrl = $"mongodb://127.0.0.1:27017/{_database}?retryWrites=true&w=majority";
        }

        private static IMongoDatabase GetDb() {
            if (_client == null) {
                _client = new MongoClient(_mongoUrl);
            }

            return _client.GetDatabase(_database);
        }

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