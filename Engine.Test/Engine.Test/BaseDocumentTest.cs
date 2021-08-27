using System;
using Xunit;
using Engine;

namespace Engine.Test
{
    public class BaseDocumentTest
    {
        [Fact]
        public void documentCreation()
        {
            string name = "Report on Search Engine Project";
            string url = "https://test.com";
            string documentId = "1";

            var doc1 = new BaseDocument(documentId);
            var doc2 = new BaseDocument(name, url);
            var doc3 = new BaseDocument(name, url, documentId);

            Assert.True(doc1.documentId == documentId);
            Assert.True(doc2.name == name && doc2.url == url);
            Assert.True(doc3.name == name && doc2.url == url && doc3.documentId == documentId);
        }
    }
}
