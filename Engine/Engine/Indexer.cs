using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using java.lang;
using TikaOnDotNet.TextExtraction;
using String = System.String;
using StringBuilder = System.Text.StringBuilder;

namespace Engine {
    public class Indexer {
        private static Queue<Document> toBeIndexed = new Queue<Document>();
        
        private static Semaphore _indexDoc = new Semaphore(1, 1);

        private static string ExtractText(string url) {
            var textExtractor = new TextExtractor();

            var webPageContents = textExtractor.Extract(new Uri(url));

            return webPageContents.Text;
        }
        
        public static void TryIndex(Document document) {
            toBeIndexed.Enqueue(document);
            IndexDocument();
        }

        private static async void IndexDocument() {
            _indexDoc.WaitOne();
            Document document = toBeIndexed.Dequeue();
            
            Index newIndex = new Index();
            
            Console.WriteLine($"Indexing document {document.position}");
                
            string text = ExtractText(document.url).Trim();

            string[] words = Utils.CleanAndExtractWords(text);
            
            for (int i = 0; i < words.Length; i++) {
                newIndex.AddWord(words[i], document, i);
            }

            await newIndex.SaveToDb();
            
            Console.WriteLine($"Done indexing document {document.position}");

            _indexDoc.Release();
        }
    }
}