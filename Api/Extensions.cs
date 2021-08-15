using Document.Dtos;
using Document.Models;
namespace Document
{
    public static class Extensions{
        public static DocDto AsDto(this Doc document)
        {
            return new DocDto
            {
                Id = document.Id,
                FilePath = document.FilePath,
                AddedOn = document.AddedOn
            };
        }
    }
}