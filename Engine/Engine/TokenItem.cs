using System.Collections.Generic;
using MongoDB.Bson;

namespace Engine {
    /// <summary>
    /// TokenItem contains the token of a particular item
    /// </summary>
    public class TokenItem {
        /// <summary>
        /// Positions contains the particular postion a token exists in a document
        /// </summary>
        public readonly List<int> Positions = new List<int>();
        /// <summary>
        /// DocumentId contains the ID of the particular document 
        /// </summary>
        public readonly string DocumentId;
        /// <summary>
        /// DocumentPosition contains the positions where the token is found
        /// </summary>
        public readonly long DocumentPosition;
        public TokenItem(int position, string documentId, long documentPosition) {
            Positions.Add(position);
            DocumentId = documentId;
            DocumentPosition = documentPosition;
        }

        /// <summary>
        /// The constructor for the TokenItem class
        /// </summary>
        /// <param name="positions">Positions contains the particular postion a token exists in documents</param>
        /// <param name="documentPosition">DocumentPosition contains the positions where the token is found</param>
        /// <param name="documentId">DocumentId contains the ID of the particular document </param>
        public TokenItem(List<int> positions, string documentId, long documentPosition) {
            Positions = positions;
            DocumentId = documentId;
            DocumentPosition = documentPosition;
        }

        /// <summary>
        /// AddPosition adds a new position
        /// </summary>
        public void AddPosition(int position) {
            Positions.Add(position);
        }

        /// <summary>
        /// GetBsonPositions converts positions to a Bson array and saves in a database
        /// </summary>
        public BsonArray GetBsonPositions() {
            var positionsArray = new BsonArray();

            foreach (var position in Positions) {
                positionsArray.Add(position);
            }

            return positionsArray;
        }
    }
}