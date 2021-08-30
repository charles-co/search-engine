using System.Collections.Generic;
using MongoDB.Bson;

namespace Engine {
    public class TokenItem {
        public readonly List<int> Positions = new List<int>();
        public readonly string DocumentId;
        public readonly long DocumentPosition;
        public TokenItem(int position, string documentId, long documentPosition) {
            Positions.Add(position);
            DocumentId = documentId;
            DocumentPosition = documentPosition;
        }
        
        public TokenItem(List<int> positions, string documentId, long documentPosition) {
            Positions = positions;
            DocumentId = documentId;
            DocumentPosition = documentPosition;
        }
        
        public void AddPosition(int position) {
            Positions.Add(position);
        }

        public BsonArray GetBsonPositions() {
            var positionsArray = new BsonArray();

            foreach (var position in Positions) {
                positionsArray.Add(position);
            }

            return positionsArray;
        }
    }
}