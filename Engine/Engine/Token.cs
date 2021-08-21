using System.Collections.Generic;

namespace Engine {
    public class Token {
        private string id;
        private List<TokenItem> documents = new List<TokenItem>();
        private int currentDocumentId = -1;

        public Token(string id) {
            this.id = id;
        }
        public void AddItem(int documentId, int position) {
            if (currentDocumentId == documentId) {
                TokenItem currentDocument = documents[documents.Count - 1];
                currentDocument.AddPosition(position);
            } else {
                currentDocumentId = documentId;
                TokenItem newDocument = new TokenItem(position);
                documents.Add(newDocument);
            }
        }
    }
}