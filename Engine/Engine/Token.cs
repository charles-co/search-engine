using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Engine {
    public class Token {
        public string word;
        public List<TokenItem> documents = new List<TokenItem>();
        private string currentDocumentId;
        public int frequency = 0;

        public Token(string word) {
            this.word = word;
        }

        public Token(string word, BsonValue[] initialDocuments, int frequency) {
            this.word = word;
            this.frequency = frequency;

            foreach (var document in initialDocuments) {
                List<int> documentPositions = BsonSerializer.Deserialize<List<int>>(document["positions"].ToJson());
                TokenItem tokenItem = new TokenItem(documentPositions, document["fileId"].ToString(),
                    document["fileIndex"].ToInt32());
                documents.Add(tokenItem);
            }
        }

        public void AddItem(Document document, int position) {
            if (currentDocumentId == document.documentId) {
                TokenItem currentDocument = documents[documents.Count - 1];
                currentDocument.AddPosition(position);
            }
            else {
                frequency++;
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

                var update = Builders<BsonDocument>.Update.Set("documents", newDocuments).Set("frequency", prevTask["frequency"].ToInt32() + frequency);

                await tokensCollection.UpdateOneAsync(getFilter, update);
            }
            else {
                var token = new BsonDocument {
                    {"word", word},
                    {"documents", GetBsonDocuments()},
                    {"frequency", frequency}
                };

                var tokensCollection = Connector.GetTokensCollection();
                
                await tokensCollection.InsertOneAsync(token);
            }
        }
    }
}