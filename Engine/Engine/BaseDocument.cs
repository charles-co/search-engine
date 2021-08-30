namespace Engine {
    public class BaseDocument {
        public string Name;
        public string Url;
        public readonly string DocumentId;

        public BaseDocument(string documentId) {
            DocumentId = documentId;
        }

        public BaseDocument(string name, string url) {
            Name = name;
            Url = url;
        }
        
        public BaseDocument(string name, string url, string documentId) : this(name, url) {
            DocumentId = documentId;
        }
    }
}