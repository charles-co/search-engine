using System.Collections.Generic;

namespace Engine {
    public class PositionPointer {
        private readonly List<int> _positions;
        private int _index;
        public readonly bool EmptyPointer;
        public readonly bool IsValid;
        
        public PositionPointer() {
            EmptyPointer = true;
        }
        
        public PositionPointer(List<int> positions, bool isValid) {
            _positions = positions;
            IsValid = isValid;
        }

        public int CurrentPosition {
            get {
                if (EmptyPointer) {
                    return -1;
                }
                
                if (_index < _positions.Count) {
                    return _positions[_index];
                }

                return -1;
            }
        }
        
        public void MoveForward() {
            _index++;
        }

        public void MoveForwardUntilGreaterThanOrEqualTo(int target) {
            while (CurrentPosition < target && _index < _positions.Count) {
                MoveForward();
            }
        }
    }
}