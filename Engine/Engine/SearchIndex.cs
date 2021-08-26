using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Engine {
    public class SearchIndex : Index {
        public SearchIndex(string[] words) {
            var filter = Builders<BsonDocument>.Filter.In("word", words);
            var tokensCollection = Connector.GetTokensCollection();
            var dbTokens = tokensCollection.Find(filter).ToList();
            
            foreach (var token in dbTokens) {
                var word = token["word"].ToString();
                var documents = token["documents"].AsBsonArray.ToArray();
                var frequency = token["frequency"].ToInt32();
                tokens[word] = new Token(word, documents, frequency);
            }
        }
    }
}