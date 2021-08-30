using System;
using System.Collections.Generic;
using System.Threading;
using TikaOnDotNet.TextExtraction;
using Exception = System.Exception;


namespace Engine {
    public static class Indexer {
        private static readonly Queue<DbDocument> ToBeIndexed = new Queue<DbDocument>();

        private static readonly Semaphore IndexDocSemaphore = new Semaphore(1, 1);

        private static string ExtractText(string url) {
            var textExtractor = new TextExtractor();
            var result = Uri.TryCreate(url, UriKind.Absolute, out var uri)
                         && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
            var contents = result ? textExtractor.Extract(uri) : textExtractor.Extract(url);
            return contents.Text;
        }

        public static void TryIndex(DbDocument dbDocument) {
            ToBeIndexed.Enqueue(dbDocument);
            IndexDocument();
        }

        private static async void IndexDocument() {
            IndexDocSemaphore.WaitOne();
            try {
                DbDocument dbDocument = ToBeIndexed.Dequeue();

                Index newIndex = new Index();

                Console.WriteLine($"Indexing document {dbDocument.Position}");

                string text = ExtractText(dbDocument.Url).Trim();

                string[] words = Utils.CleanAndExtractWords(text);

                for (int i = 0; i < words.Length; i++) {
                    newIndex.AddWord(words[i], dbDocument, i);
                }

                await newIndex.SaveToDb();

                Console.WriteLine($"Done indexing document {dbDocument.Position}");
            }
            catch (Exception e) {
                Console.WriteLine(e.StackTrace, e.Message); //TODO: Log to file
            }

            IndexDocSemaphore.Release();
        }
    }
}