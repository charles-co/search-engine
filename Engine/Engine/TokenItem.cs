using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;

namespace Engine {
    public class TokenItem {
        public List<int> positions = new List<int>();
        public string documentId;
        public long documentPosition;
        public TokenItem(int position, string documentId, long documentPosition) {
            positions.Add(position);
            this.documentId = documentId;
            this.documentPosition = documentPosition;
        }
        
        public void AddPosition(int position) {
            positions.Add(position);
        }

        public BsonArray GetBsonPositions() {
            var positionsArray = new BsonArray();

            foreach (var position in positions) {
                positionsArray.Add(position);
            }

            return positionsArray;
        }
    }
}