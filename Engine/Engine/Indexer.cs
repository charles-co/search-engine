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
        private static Queue<DbDocument> toBeIndexed = new Queue<DbDocument>();
        
        private static Semaphore _indexDoc = new Semaphore(1, 1);

        private static string ExtractText(string url) {
            var textExtractor = new TextExtractor();

            var webPageContents = textExtractor.Extract(new Uri(url));

            return webPageContents.Text;
        }
        
        public static void TryIndex(DbDocument dbDocument) {
            toBeIndexed.Enqueue(dbDocument);
            IndexDocument();
        }

        private static async void IndexDocument() {
            _indexDoc.WaitOne();
            DbDocument dbDocument = toBeIndexed.Dequeue();
            
            Index newIndex = new Index();
            
            Console.WriteLine($"Indexing document {dbDocument.position}");
                
            string text = ExtractText(dbDocument.url).Trim();

            string[] words = Utils.CleanAndExtractWords(text);
            
            for (int i = 0; i < words.Length; i++) {
                newIndex.AddWord(words[i], dbDocument, i);
            }

            await newIndex.SaveToDb();
            
            Console.WriteLine($"Done indexing document {dbDocument.position}");

            _indexDoc.Release();
        }
    }
}