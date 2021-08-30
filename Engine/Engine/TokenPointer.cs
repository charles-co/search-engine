using System.Collections.Generic;
using Priority_Queue;

namespace Engine {
    /// <summary>
    /// Token Pointer class is used for the linear mapping algorithm
    /// </summary>
    public class TokenPointer : StablePriorityQueueNode {
        private readonly List<TokenItem> _targets;
        public readonly Token Token;
        private int _index;
        public readonly bool EmptyPointer;
        
        public TokenItem Target => _index < _targets.Count ? _targets[_index] : null;

        public TokenPointer() {
            EmptyPointer = true;
        }

        public TokenPointer(Token targetToken) {
            Token = targetToken;
            _targets = targetToken.Documents;
        }

        /// <summary>
        /// Move Forward function is used to move the current index by 1
        /// </summary>
        public bool MoveForward() {
            if (_index + 1 < _targets.Count) {
                _index++;
                return true;
            }

            return false;
        }
    }
}