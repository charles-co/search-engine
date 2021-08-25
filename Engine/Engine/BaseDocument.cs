namespace Engine {
    public class BaseDocument {
        public string name;
        public string url;
        public string documentId;

        public BaseDocument(string documentId) {
            this.documentId = documentId;
        }

        public BaseDocument(string name, string url) {
            this.name = name;
            this.url = url;
        }
        
        public BaseDocument(string name, string url, string documentId) {
            this.name = name;
            this.url = url;
            this.documentId = documentId;
        }
    }
}