using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Engine {
    public class Token {
        private readonly string _word;
        public readonly List<TokenItem> Documents = new List<TokenItem>();
        private string _currentDocumentId;
        public int Frequency;

        public Token(string word) {
            _word = word;
        }

        public Token(string word, BsonValue[] initialDocuments, int frequency) {
            _word = word;
            Frequency = frequency;

            foreach (var document in initialDocuments) {
                var documentPositions = BsonSerializer.Deserialize<List<int>>(document["positions"].ToJson());
                var tokenItem = new TokenItem(documentPositions, document["fileId"].ToString(),
                    document["fileIndex"].ToInt32());
                Documents.Add(tokenItem);
            }
        }

        public void AddItem(DbDocument dbDocument, int position) {
            if (_currentDocumentId == dbDocument.DocumentId) {
                var currentDocument = Documents[Documents.Count - 1];
                currentDocument.AddPosition(position);
            }
            else {
                Frequency++;
                _currentDocumentId = dbDocument.DocumentId;
                var newDocument = new TokenItem(position, dbDocument.DocumentId, dbDocument.Position);
                Documents.Add(newDocument);
            }
        }

        private BsonArray GetBsonDocuments() {
            var documentsArray = new BsonArray();
            foreach (var document in Documents) {
                var item = new BsonDocument {
                    {"fileId", document.DocumentId},
                    {"fileIndex", document.DocumentPosition},
                    {"positions", document.GetBsonPositions()},
                };

                documentsArray.Add(item);
            }

            return documentsArray;
        }
        
        public async Task SaveSelfToDb() {
            var getFilter = Builders<BsonDocument>.Filter.Eq("word", _word);
            var tokensCollection = Connector.GetTokensCollection();
            var prevToken = tokensCollection.Find(getFilter).FirstOrDefault();

            if (prevToken != null) {
                var taskBson = prevToken.ToBsonDocument();
                var previousDocuments = BsonSerializer.Deserialize<BsonArray>(taskBson["documents"].ToJson());
                var newDocuments = previousDocuments.AddRange(GetBsonDocuments());
                
                var update = Builders<BsonDocument>.Update.Set("documents", newDocuments).Set("frequency", prevToken["frequency"].ToInt32() + Frequency);

                await tokensCollection.UpdateOneAsync(getFilter, update);
            }
            else {
                var token = new BsonDocument {
                    {"word", _word},
                    {"documents", GetBsonDocuments()},
                    {"frequency", Frequency}
                };
                
                await tokensCollection.InsertOneAsync(token);
            }
        }
    }
}