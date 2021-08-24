using System.Collections.Generic;

namespace Engine {
    public class PositionPointer {
        public List<int> positions;
        public int index = 0;

        public PositionPointer(List<int> positions) {
            this.positions = positions;
        }

        public int currentPosition {
            get {
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
            while (currentPosition < target) {
                moveForward();
            }
        }
    }
}