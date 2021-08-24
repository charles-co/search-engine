using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using java.util;
using MongoDB.Bson;
using Priority_Queue;

namespace Engine {
    public class Querier {
        private StablePriorityQueue<TokenPointer> _queue;
        private StablePriorityQueue<ResultDocument> _scores;
        private long documentsCount;

        public async void Search(string query) {
            var cleanedWords = Utils.CleanAndExtractWords(query);
            
            documentsCount = await Connector.GetDocumentsCollection().CountDocumentsAsync(new BsonDocument());

            Index searchIndex = new SearchIndex(cleanedWords);
            
            GeneratePointers(searchIndex, cleanedWords);
            LinearMap();
        }
        
        private void GeneratePointers(Index targetIndex, string [] words) {
            _queue = new StablePriorityQueue<TokenPointer>(words.Length);
            _scores = new StablePriorityQueue<ResultDocument>(words.Length);
            
            foreach (var word in words) {
                if (targetIndex.tokens.ContainsKey(word)) {
                    var pointer = new TokenPointer(targetIndex.tokens[word]);
                    _queue.Enqueue(pointer, pointer.target.documentPosition);   
                }
            }
        }

        private void LinearMap() {
            List<TokenPointer> smallestPointers = ExtractSmallest(_queue);
            
            ScorePointers(smallestPointers);
                
            foreach (var pointer in smallestPointers) {
                if (pointer.moveForward()) {
                    _queue.Enqueue(pointer, pointer.target.documentPosition);
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
            
            double d = ScoreConsecutiveWords(pointers);

            documentScore += d;
            
            string targetDocumentId = pointers[0].target.documentId;
            Console.WriteLine($"{d} ${targetDocumentId}");
            _scores.Enqueue(new ResultDocument(targetDocumentId, (float) documentScore), (float) documentScore);
        }

        private double ScoreConsecutiveWords(List<TokenPointer> pointers) {
            //Total consecutive count
            int consecutiveCount = 0;
            List<PositionPointer> positionPointers = new List<PositionPointer>();

            foreach (var pointer in pointers) {
                //Add position arrays to pointers array
                positionPointers.Add(new PositionPointer(pointer.target.positions));
            }
            
            while (true) {
                bool hasInvalidPointer = false;
                int currentRunOn = 0;
                
                //This is to store the first point at which our run on failed so that we can increase everything behind that
                int firstBreakPoint = 0;
                bool hasSetFirstBreakPoint = false;
                
                for (int i = 0; i < positionPointers.Count; i++) {
                    var positionPointer = positionPointers[i];

                    //If we have invalid just break out of loop to save resources
                    if (positionPointer.currentPosition == -1) {
                        hasInvalidPointer = positionPointer.currentPosition == -1;
                        break;
                    }
                    
                    if (positionPointer.currentPosition != -1) {
                        //If first item start the run
                        if (i == 0) {
                            currentRunOn = 1;
                        }
                        else {
                            //Else check if previous item is directly before current item
                            var prevPointer = positionPointers[i - 1];
                            if (positionPointer.currentPosition - 1 == prevPointer.currentPosition) {
                                currentRunOn++;
                            }
                            //Else if current item position is behind previous item
                            //(which it shouldn't be because of the order of the words should match the query)
                            //Then move the current item forward until it's ahead of the previous item
                            else if (positionPointer.currentPosition < prevPointer.currentPosition) {
                                positionPointer.moveForwardUntilGreaterThanOrEqualTo(prevPointer.currentPosition);
                                if (positionPointer.currentPosition - 1 == prevPointer.currentPosition) {
                                    currentRunOn++;
                                }    
                            }
                            else {
                                //If we have a runon add it to our consecutive count
                                if (currentRunOn > 1) {
                                    consecutiveCount += currentRunOn;
                                    currentRunOn = 1;
                                }
                                
                                //If we've not set our first breakpoint set it
                                if(!hasSetFirstBreakPoint) {
                                    firstBreakPoint = i - 1;
                                    hasSetFirstBreakPoint = true;
                                }
                            }
                        }
                    }
                }

                //If there's a residual currentRunon i.e there was a runon towards the end of the string just add it to consecutive count
                if (currentRunOn > 1) {
                    consecutiveCount += currentRunOn;
                }

                //If we haven't set our first breakpoint that means we never broke so move both token positions forward
                if (!hasSetFirstBreakPoint) {
                    firstBreakPoint = positionPointers.Count - 1;
                }

                for (int i = 0; i <= firstBreakPoint; i++) {
                    var positionPointer = positionPointers[i];
                    positionPointer.moveForward();
                }

                if (hasInvalidPointer) {
                    break;
                }
            }

            return consecutiveCount;
        }

        private List<TokenPointer> ExtractSmallest(StablePriorityQueue<TokenPointer> pointers) {
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

    class ResultDocument : StablePriorityQueueNode {
        public string documentId;
        public float score;

        public ResultDocument(string documentId, float score) {
            this.documentId = documentId;
            this.score = score;
        }
    }
}