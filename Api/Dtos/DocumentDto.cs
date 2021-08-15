using System;
namespace Document.Dtos
{
    public record DocDto {
        public Guid Id { get; init; }
        public string FilePath { get; set; }
        public DateTimeOffset AddedOn { get; init; }
    }
}