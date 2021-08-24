using System;
using System.Threading.Tasks;
using API.Models;

namespace API.Core
{
    public static class IndexingService
    {
        
        // [DisableConcurrentExecution(timeoutInSeconds: 60 * 10)]
        public static async Task Index(Document doc)
        {
            Console.WriteLine("Indexing");
            // uint lastId = await InvertedIndex.GetLastId();
            // Indexer indexer = new Indexer(lastId);
            //
            // Console.WriteLine("indexing file id: {0}", doc.Id);
            // await indexer.Index(doc.Url, doc.Id);
            // Console.WriteLine("file {0} finished indexing", doc.Id);
        }
    }
}