using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Document.Models;
using Document.Data;


namespace Document.Repositories
{
    public class DocumentRepository: IDocumentRepository
    {
        private readonly IDataContext _context;

        public DocumentRepository(IDataContext context)
        {
            _context = context;
        }

        public async Task Add(Doc document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Guid id)
        {
            var docToRemove = await _context.Documents.FindAsync(id);
            if (docToRemove == null)
                throw new NullReferenceException();
            _context.Documents.Remove(docToRemove);
            await _context.SaveChangesAsync();
        }

        public async Task<Doc> Get(Guid id)
        {
            return await _context.Documents.FindAsync(id);
        }

        public async Task<IEnumerable<Doc>> GetAll()
        {
            return await _context.Documents.ToListAsync();
        }
    }
}