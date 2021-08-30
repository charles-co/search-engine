using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using java.lang;
using java.util;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Priority_Queue;
using Exception = System.Exception;
using Math = System.Math;

namespace Engine {
    public class Querier {
        private StablePriorityQueue<TokenPointer> _queue;
        private SimplePriorityQueue<ScoreDocumentNode> _scores;
        private long _documentsCount;
        private BaseDocument [] _resultDocuments;
        private List<TokenPointer> arrangedPointers = new List<TokenPointer>();

        public static string[] GetPastQueries(string query) {
            var getQueriesFilter = Builders<BsonDocument>.Filter.Regex("query", new BsonRegularExpression($"^{query}"));
            var savedQueriesCollection = Connector.GetSavedQueriesCollection();
            var savedQueries = savedQueriesCollection.Find(getQueriesFilter).Limit(10).SortByDescending(x => x["count"]).ToList();
            var queries = new string[savedQueries.Count];

            for (int i = 0; i < savedQueries.Count; i++) {
                queries[i] = savedQueries[i].ToBsonDocument()["query"].ToString();
            }

            return queries;
        }
        
        public async Task<BaseDocument []> Search(string query) {
            try {
                var cleanedWords = Utils.CleanAndExtractWords(query);

                DateTime start = DateTime.Now;
                Index searchIndex = new SearchIndex(cleanedWords);
                Console.WriteLine($"Load words {(DateTime.Now - start).TotalMilliseconds}");
                
                if (searchIndex.tokens.Count != 0) {
                    _documentsCount = await Connector.GetDocumentsCollection().CountDocumentsAsync(new BsonDocument());
                    start = DateTime.Now;
                    GeneratePointers(searchIndex, cleanedWords);
                    LinearMap();
                    Console.WriteLine($"Generate scores {(DateTime.Now - start).TotalMilliseconds}");
                    start = DateTime.Now;
                    await FetchDocumentDetails();
                    Console.WriteLine($"Fetch documents {(DateTime.Now - start).TotalMilliseconds}");

                    Task.Run(() => SaveQueryToDb(query));
                    
                    return _resultDocuments;
                }

            }
            catch (Exception e) {
                Console.WriteLine(e.StackTrace, e.Message); //TODO: Log to file
            }

            _resultDocuments = new BaseDocument[0];
            return _resultDocuments;
        }

        private async void SaveQueryToDb(string query) {
            var getFilter = Builders<BsonDocument>.Filter.Eq("query", query);
            var savedQueriesCollection = Connector.GetSavedQueriesCollection();
            var prevQuery = savedQueriesCollection.Find(getFilter).FirstOrDefault();

            if (prevQuery != null) {
                var queryBson = prevQuery.ToBsonDocument();
                int count = queryBson["count"].ToInt32();
                var update = Builders<BsonDocument>.Update.Set("count", count + 1);
                await savedQueriesCollection.UpdateOneAsync(getFilter, update);
            }
            else {
                var token = new BsonDocument {
                    {"query", query},
                    {"count", 1},
                };
                
                await savedQueriesCollection.InsertOneAsync(token);
            }
            
            Console.WriteLine($"{query} saved to saved queries collection");
        }
        
        private async Task FetchDocumentDetails() {
            string[] resultIds = new string[_scores.Count];
            _resultDocuments = new BaseDocument[_scores.Count];
            
            List<FilterDefinition<BsonDocument>> filtersList = new List<FilterDefinition<BsonDocument>>();
            
            while (_scores.Count > 0) {
                var currentDoc = _scores.Dequeue();
                resultIds[_scores.Count] = currentDoc.DocumentId;
                _resultDocuments[_scores.Count] = new BaseDocument(currentDoc.DocumentId);
                
                var getFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(currentDoc.DocumentId));
                filtersList.Add(getFilter);
            }

            var filter = Builders<BsonDocument>.Filter.Or(filtersList);
            var documents = await Connector.GetDocumentsCollection().FindAsync(filter);
            var bson = documents.ToList();

            Dictionary<string, BsonDocument> docIdToValueMapping = new Dictionary<string, BsonDocument>();

            foreach (var doc in bson) {
                var currentId = doc["_id"].ToString();
                docIdToValueMapping[currentId] = doc;
            }

            for (int i = 0; i < resultIds.Length; i++) {
                var id = resultIds[i];
                var values = docIdToValueMapping[id];
                _resultDocuments[i].name = values["name"].ToString();
                _resultDocuments[i].url = values["url"].ToString();
            }
        }

        private void GeneratePointers(Index targetIndex, string [] words) {
            _queue = new StablePriorityQueue<TokenPointer>(words.Length);
            _scores = new SimplePriorityQueue<ScoreDocumentNode>();
            
            foreach (var word in words) {
                if (targetIndex.tokens.ContainsKey(word)) {
                    var pointer = new TokenPointer(targetIndex.tokens[word]);
                    _queue.Enqueue(pointer, pointer.target.documentPosition);
                    arrangedPointers.Add(pointer);
                }
                else {
                    arrangedPointers.Add(new TokenPointer());
                }
            }
        }

        private void LinearMap() {
            List<TokenPointer> smallestPointers = ExtractSmallest();
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

                double tfIdf = termFrequency * Math.Log(_documentsCount / documentsWithTerm, 2);
                
                documentScore += tfIdf;
            }

            double d = ScoreConsecutiveWords(pointers[0].target.documentId);

            documentScore += d;
            
            string targetDocumentId = pointers[0].target.documentId;
            _scores.Enqueue(new ScoreDocumentNode(targetDocumentId), (float) documentScore);
        }

        private double ScoreConsecutiveWords(string targetDocumentId) {
            //Total consecutive count
            int consecutiveCount = 0;
            List<PositionPointer> positionPointers = new List<PositionPointer>();
            
            foreach (var pointer in arrangedPointers) {
                if (pointer.emptyPointer) {
                    positionPointers.Add(new PositionPointer());   
                }
                else {
                    //Add position arrays to pointers array
                    positionPointers.Add(new PositionPointer(pointer.target.positions, pointer.target.documentId == targetDocumentId));   
                }
            }

            while (true) {
                bool hasInvalidPointer = false;
                int currentRunOn = 0;
                
                //This is to store the first point at which our run on failed so that we can increase everything behind that
                int firstBreakPoint = 0;
                bool hasSetFirstBreakPoint = false;
                
                for (int i = 0; i < positionPointers.Count; i++) {
                    var positionPointer = positionPointers[i];

                    if (positionPointer.emptyPointer || !positionPointer.isValid) {
                        if (currentRunOn > 1) {
                            consecutiveCount += currentRunOn;
                            currentRunOn = 1;
                        }
                        
                        if(!hasSetFirstBreakPoint) {
                            firstBreakPoint = Math.Max(i - 1, 0);
                            hasSetFirstBreakPoint = true;
                        }
                        continue;
                    }
                    
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
                    if (!positionPointer.emptyPointer && positionPointer.isValid) {
                        positionPointer.moveForward();
                    }
                }

                if (hasInvalidPointer) {
                    break;
                }
            }

            return consecutiveCount;
        }

        private List<TokenPointer> ExtractSmallest() {
            var smallestPointers = new List<TokenPointer>();

            while (true) {
                smallestPointers.Add(_queue.Dequeue());

                if (_queue.Count == 0) {
                    break;
                }

                var leastPointer = smallestPointers[0];
                var leastPointerInQueue = _queue.First;

                if (leastPointer.target.documentPosition != leastPointerInQueue.target.documentPosition) {
                    break;
                }
            }
            
            return smallestPointers;
        }
    }
}