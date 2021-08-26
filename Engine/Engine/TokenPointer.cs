using System.Collections.Generic;
using Priority_Queue;

namespace Engine {
    public class TokenPointer : StablePriorityQueueNode {
        public List<TokenItem> targets;
        public Token token;
        public int index = 0;

        public TokenItem target {
            get {
                if (index < targets.Count) {
                    return targets[index];
                }

                return null;
            }
        }

        public TokenPointer(Token targetToken) {
            token = targetToken;
            targets = targetToken.documents;
        }

        public bool moveForward() {
            if (index + 1 < targets.Count) {
                index++;
                return true;
            }

            return false;
        }
    }
}