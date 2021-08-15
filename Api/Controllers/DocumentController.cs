using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Document.Dtos;
using Document.Models;
using Document.Repositories;

namespace Document.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentRepository _documentRepository;
        public DocumentController(IDocumentRepository documentRepository)
        {
            _documentRepository = documentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doc>>> GetDocuments()
        {
            var documents = (await _documentRepository.GetAll())
                            .Select(document => document.AsDto());
            return Ok(documents);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Doc>> GetDocument(Guid id)
        {
            var document = (await _documentRepository.Get(id));
            if (document == null)
                return NotFound();
            return Ok(document.AsDto());
        }

        [HttpPost]
        public async Task<ActionResult> CreateDocument(CreateDocumentDto createDocumentDto)
        {
            Doc document = new()
            {   
                Id = Guid.NewGuid(),
                FilePath = createDocumentDto.FilePath,
                AddedOn = DateTimeOffset.UtcNow
            };
            await _documentRepository.Add(document);
            return CreatedAtAction(nameof(GetDocument), new { id = document.Id }, document.AsDto());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDocument(Guid id)
        {
            await _documentRepository.Delete(id);
            return Ok();
        }

    }
}