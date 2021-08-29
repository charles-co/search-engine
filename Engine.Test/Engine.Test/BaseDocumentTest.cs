using System;
using Xunit;
using Xunit.Abstractions;
using Engine;

namespace Engine.Test
{
    public class BaseDocumentTest
    {
        private readonly ITestOutputHelper _output;

        public BaseDocumentTest(ITestOutputHelper output){
            _output = output;
        }

        [Fact]
        public void DocumentCreation()
        {
           const string name = "Report on Search Engine Project";
           const string url = "https://test.com";
           const string documentId = "1";

           var doc1 = new BaseDocument(documentId);
           var doc2 = new BaseDocument(name, url);
           var doc3 = new BaseDocument(name, url, documentId);

            _output.WriteLine("Test running.");

            Assert.True(doc1.documentId == documentId);
            Assert.True(doc2.name == name && doc2.url == url);
            Assert.True(doc3.name == name && doc2.url == url && doc3.documentId == documentId);
        }
    }
}
