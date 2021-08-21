using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Engine {
    public class Token {
        public string word;
        private List<TokenItem> documents = new List<TokenItem>();
        private string currentDocumentId;
        private int frequency = 0;

        public Token(string word) {
            this.word = word;
        }

        public void AddItem(Document document, int position) {
            frequency++;

            if (currentDocumentId == document.documentId) {
                TokenItem currentDocument = documents[documents.Count - 1];
                currentDocument.AddPosition(position);
            }
            else {
                currentDocumentId = document.documentId;
                TokenItem newDocument = new TokenItem(position, document.documentId, document.position);
                documents.Add(newDocument);
            }
        }

        public BsonArray GetBsonDocuments() {
            var documentsArray = new BsonArray();
            foreach (var document in documents) {
                var item = new BsonDocument {
                    {"fileId", document.documentId},
                    {"fileIndex", document.documentPosition},
                    {"positions", document.GetBsonPositions()},
                    {"frequency", frequency}
                };

                documentsArray.Add(item);
            }

            return documentsArray;
        }
        
        public async Task SaveSelfToDb() {
            var getFilter = Builders<BsonDocument>.Filter.Eq("word", word);
            var prevTask = Connector.GetTokensCollection().Find(getFilter).FirstOrDefault();

            if (prevTask != null) {
                var taskBson = prevTask.ToBsonDocument();
                var previousDocuments = BsonSerializer.Deserialize<BsonArray>(taskBson["documents"].ToJson());
                var newDocuments = previousDocuments.AddRange(GetBsonDocuments());
                
                var tokensCollection = Connector.GetTokensCollection();

                var update = Builders<BsonDocument>.Update.Set("documents", newDocuments);

                await tokensCollection.UpdateOneAsync(getFilter, update);
            }
            else {
                var token = new BsonDocument {
                    {"word", word},
                    {"documents", GetBsonDocuments()}
                };

                var tokensCollection = Connector.GetTokensCollection();

                Console.WriteLine($"Saving token {word}");

                await tokensCollection.InsertOneAsync(token);
            }

            Console.WriteLine($"Saved token {word}");
        }
    }
}