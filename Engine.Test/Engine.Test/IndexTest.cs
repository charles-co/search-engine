using System.Threading;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Xunit;
using Xunit.Abstractions;
using Engine;

namespace Engine.Test
{
    public class IndexTest
    {
        private static readonly string _name = "Report for testing";
        private static readonly string _url = "../../fixtures/CSC326.docx";

        private readonly ITestOutputHelper _output;
        public DbDocument Dbdoc { get; private set; }
        FilterDefinition<BsonDocument> _filter = Builders<BsonDocument>.Filter.Eq("name", _name);

        public IndexTest(ITestOutputHelper output)
        {
            _output = output;
        }

        private void SetupDb()
        {
            
            Connector.SetTestMode();
            var dColl = Connector.GetDocumentsCollection();
            dColl.DeleteMany(new BsonDocument());
            var docs = dColl.AsQueryable().ToList();
            
            _output.WriteLine($"Before setup, no of docs: {docs.Count}");
            
            DbDocument.IndexDocument(_name, _url);
            Thread.Sleep(5000);
            var document = Connector.GetDocumentsCollection().Find(_filter).SingleOrDefault();
            _output.WriteLine($"{document.GetValue("_id")}");
            document.Remove("_id");
            Dbdoc = BsonSerializer.Deserialize<DbDocument>(document);
            
            docs = dColl.AsQueryable().ToList();
            _output.WriteLine($"After setup, no of docs: {docs.Count}");
        }

        private void TearDown()
        {
            var dColl = Connector.GetDocumentsCollection();
            var docs = dColl.AsQueryable().ToList();
            _output.WriteLine($"Before teardown, no of docs: {docs.Count}");
            dColl.DeleteMany(new BsonDocument());
            docs = dColl.Find(_filter).ToList();
            _output.WriteLine($"Ended, no of docs: {docs.Count}");  
            
            var tColl = Connector.GetTokensCollection();
            var tokens = tColl.AsQueryable().ToList();
            _output.WriteLine($"Before teardown, no of tokens: {tokens.Count}");
            tColl.DeleteMany(new BsonDocument());
            tokens = tColl.Find(new BsonDocument()).ToList();
            _output.WriteLine($"Ended, no of tokens: {tokens.Count}"); 
        }

        [Fact]
        public void IndexDocument()
        {
            SetupDb();
            var document = Connector.GetDocumentsCollection().Find(_filter).SingleOrDefault();
            var token = Connector.GetTokensCollection().AsQueryable().FirstOrDefault();
            _output.WriteLine($"{token}, {document}");
            Assert.NotNull(document);
            Assert.NotNull(token);
            TearDown();
        }

    }
}