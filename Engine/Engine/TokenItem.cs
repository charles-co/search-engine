using System;
using System.Collections;
using System.Collections.Generic;

namespace Engine {
    public class TokenItem {
        private List<int> positions = new List<int>();

        public TokenItem(int position) {
            positions.Add(position);
        }
        
        public void AddPosition(int position) {
            positions.Add(position);
        }
    }
}