using System;
using System.Collections.Generic;
using System.Linq;
using java.lang;
using TikaOnDotNet.TextExtraction;
using String = System.String;
using StringBuilder = System.Text.StringBuilder;

namespace Engine {
    public class Indexer {
        private const string StopWords = "i me my myself we our ours ourselves you youre youve youll youd your yours yourself yourselves he him his himself she shes her hers herself it its its itself they them their theirs themselves what which who whom this that thatll these those am is are was were be been being have has had having do does did doing a an the and but if or because as until while of at by for with about against between into through during before after above below to from up down in out on off over under again further then once here there when where why how all any both each few more most other some such no nor not only own same so than too very s t can will just don dont should shouldve now d ll m o re ve y ain aren arent couldn couldnt didn didnt doesn doesnt hadn hadnt hasn hasnt haven havent isn isnt ma mightn mightnt mustn mustnt needn neednt shan shant shouldn shouldnt wasn wasnt weren werent wont wouldn wouldnt";
        private static readonly HashSet<string> StopWordsHash = new HashSet<string>(StopWords.Split(' '));
        private static Queue<Document> toBeIndexed = new Queue<Document>();
        private static string ExtractText(string url) {
            var textExtractor = new TextExtractor();

            var webPageContents = textExtractor.Extract(new Uri(url));

            return webPageContents.Text;
        }

        public static Index currentIndex = new Index();
        
        private static string NormalizeWhiteSpaceAndRemovePunctuation(string text) {
            StringBuilder output = new StringBuilder();
            bool skipped = false;

            foreach (char c in text) {
                if (char.IsWhiteSpace(c)) {
                    if (!skipped) {
                        output.Append(' ');
                        skipped = true;
                    }
                }
                else if (char.IsPunctuation(c)) {
                    if (!skipped) {
                        output.Append(' ');
                        skipped = true;
                    }
                }
                else {
                    skipped = false;
                    output.Append(c);
                }
            }

            return output.ToString();
        }

        private static bool IsStopWord(string word) {
            return StopWordsHash.Contains(word);
        }

        private static string [] SplitAndRemoveStopWords(string text) {
            string[] words = text.Split(' ');

            return words.Where(word => !IsStopWord(word)).ToArray();
        }

        private static void StemWords(string [] validWords) {
            Stemmer stemmer = new Stemmer();

            for (int i = 0; i < validWords.Length; i++) {
                validWords[i] = stemmer.StemWord(validWords[i]);
            }         
        }

        public static void TryIndex(Document document) {
            toBeIndexed.Enqueue(document);

            if (toBeIndexed.Count == 1) {
                IndexDocument();
            }
        }

        private static void IndexDocument() {
            Document document = toBeIndexed.Dequeue();
            
            Console.WriteLine($"Indexing document {document.documentId}");
                
            string text = ExtractText(document.url).Trim();

            string normalizedText = NormalizeWhiteSpaceAndRemovePunctuation(text.ToLower());

            string[] words = SplitAndRemoveStopWords(normalizedText);
            
            StemWords(words);
            
            int position = 0;
                
            for (int i = 0; i < words.Length; i++) {
                currentIndex.AddWord(words[i], document.documentId, position);
                position += (words[i].Length);
            }
            
            Console.WriteLine($"Done indexing document {document.documentId}");

            if (toBeIndexed.Count > 0) {
                IndexDocument();
            }
        }
    }
}