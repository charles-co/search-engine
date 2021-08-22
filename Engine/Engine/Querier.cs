using System;
using System.Collections.Generic;
using Priority_Queue;

namespace Engine {
    public class Querier {
        private FastPriorityQueue<TokenPointer> _queue;
        
        public void Search(string query) {
            var cleanedWords = Utils.CleanAndExtractWords(query);
            Index searchIndex = new SearchIndex(cleanedWords);
            GeneratePointers(searchIndex, cleanedWords);
        }

        private void GeneratePointers(Index targetIndex, string [] words) {
            _queue = new FastPriorityQueue<TokenPointer>(words.Length);
            
            foreach (var word in words) {
                var pointer = new TokenPointer(targetIndex.tokens[word]);
                _queue.Enqueue(pointer, pointer.target.documentPosition);
            }
        }
    }
}