using System.Collections;
using System.Collections.Generic;

namespace Engine {
    public class TokenItem {
        private int delta;
        private List<int> positions;
        private int frequency;

        public TokenItem(int position) {
            positions.Add(position);
        }
        public void AddPosition(int position) {
            int lastItem = positions[positions.Count - 1];
            //Delta system
            positions.Add(position - lastItem);
        }
    }
}