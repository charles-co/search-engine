using System.Collections.Generic;
using System.Threading.Tasks;
using Document.Models;
using System;

namespace Document.Repositories
{
    public interface IDocumentRepository
    {
        Task<Doc> Get(Guid id);
        Task<IEnumerable<Doc>> GetAll();
        Task Add(Doc document);
        Task Delete(Guid id);
    }
}