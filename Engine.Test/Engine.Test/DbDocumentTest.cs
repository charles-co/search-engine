using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;
using Xunit.Abstractions;
using TikaOnDotNet.TextExtraction;

namespace Engine.Test
{
    public class DbDocumentTest
    {
        private readonly string _name = "Report for testing";
        private readonly string _url = "../../fixtures/CSC326.docx";
        
        private readonly ITestOutputHelper _output;

        public DbDocumentTest(ITestOutputHelper output){
            _output = output;
        }

        [Fact]
        public void IndexDocument()
        {
            var filter = Builders<BsonDocument>.Filter.Eq("name", _name);
            Connector.SetTestMode();
            var dColl = Connector.GetDocumentsCollection();
            DbDocument.IndexDocument(_name, _url);
            Thread.Sleep(5000);
            var doc = dColl.Find(filter).SingleOrDefault();
            _output.WriteLine($"Here's your doc: {doc}");
            Assert.NotNull(doc);
            var docs = dColl.AsQueryable().ToList();
            _output.WriteLine($"Before teardown, no of docs: {docs.Count}");
            dColl.DeleteMany(filter);
            docs = dColl.Find(filter).ToList();
            _output.WriteLine($"Ended, no of docs: {docs.Count}")
        }
    }
}