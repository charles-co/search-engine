using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using java.util;
using MongoDB.Bson;
using Priority_Queue;

namespace Engine {
    public class Querier {
        private FastPriorityQueue<TokenPointer> _queue;
        private FastPriorityQueue<ResultDocument> _scores;
        private long documentsCount;
        
        public async void Search(string query) {
            var cleanedWords = Utils.CleanAndExtractWords(query);
            DateTime start = DateTime.Now;
            documentsCount = await Connector.GetDocumentsCollection().CountDocumentsAsync(new BsonDocument());
            Console.WriteLine($"Time to count docs: {(DateTime.Now - start).TotalSeconds}");
            
            start = DateTime.Now;
            Index searchIndex = new SearchIndex(cleanedWords);
            Console.WriteLine($"Time to load words: {(DateTime.Now - start).TotalSeconds}");
            
            start = DateTime.Now;
            GeneratePointers(searchIndex, cleanedWords);
            LinearMap();
            Console.WriteLine($"Time to load generate results: {(DateTime.Now - start).TotalSeconds}");
            Console.WriteLine(_scores.First);
        }

        private void GeneratePointers(Index targetIndex, string [] words) {
            _queue = new FastPriorityQueue<TokenPointer>(words.Length);
            _scores = new FastPriorityQueue<ResultDocument>(words.Length);
            
            foreach (var word in words) {
                var pointer = new TokenPointer(targetIndex.tokens[word]);
                _queue.Enqueue(pointer, pointer.target.documentPosition);
            }
        }

        private void LinearMap() {
            List<TokenPointer> smallestPointers = ExtractSmallest(_queue);

            ScorePointers(smallestPointers);

            foreach (var smallestPointer in smallestPointers) {
                if (smallestPointer.moveForward()) {
                    _queue.Enqueue(smallestPointer, smallestPointer.target.documentPosition);
                };
            }

            if (_queue.Count > 0) {
                LinearMap();
            }
        }

        private void ScorePointers(List<TokenPointer> pointers) {
            double documentScore = 0;

            foreach (var tokenPointer in pointers) {
                var target = tokenPointer.target;
                double termFrequency = target.positions.Count;
                double documentsWithTerm = tokenPointer.token.frequency;

                double tfIdf = termFrequency * Math.Log(documentsCount / documentsWithTerm, 2);
                
                documentScore += tfIdf;
            }
            

            string targetDocumentId = pointers[0].target.documentId;
            _scores.Enqueue(new ResultDocument(targetDocumentId, (float) documentScore), (float) documentScore);
        }

        private List<TokenPointer> ExtractSmallest(FastPriorityQueue<TokenPointer> pointers) {
            var smallestPointers = new List<TokenPointer>();

            while (true) {
                smallestPointers.Add(pointers.Dequeue());

                if (pointers.Count == 0) {
                    break;
                }

                var leastPointer = smallestPointers[0];
                var leastPointerInQueue = pointers.First;

                if (leastPointer.target.documentPosition != leastPointerInQueue.target.documentPosition) {
                    break;
                }
            }

            return smallestPointers;
        }
    }

    class ResultDocument : FastPriorityQueueNode {
        public string documentId;
        public float score;

        public ResultDocument(string documentId, float score) {
            this.documentId = documentId;
            this.score = score;
        }
    }
}