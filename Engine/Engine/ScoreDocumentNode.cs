using Priority_Queue;

namespace Engine {
    public class ScoreDocumentNode : StablePriorityQueueNode {
        public readonly string DocumentId;

        public ScoreDocumentNode(string documentId) {
            this.DocumentId = documentId;
        }
    }
}