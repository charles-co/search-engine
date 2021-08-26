using System;
using System.Collections.Generic;

namespace Engine {
    public class PositionPointer {
        public List<int> positions;
        public int index = 0;
        public bool emptyPointer = false;
        public bool isValid = false;
        
        public PositionPointer() {
            emptyPointer = true;
        }
        
        public PositionPointer(List<int> positions, bool isValid) {
            this.positions = positions;
            this.isValid = isValid;
        }

        public int currentPosition {
            get {
                if (emptyPointer) {
                    return -1;
                }
                
                if (index < positions.Count) {
                    return positions[index];
                }

                return -1;
            }
        }
        
        public void moveForward() {
            index++;
        }

        public void moveForwardUntilGreaterThanOrEqualTo(int target) {
            while (currentPosition < target && index < positions.Count) {
                moveForward();
            }
        }
    }
}