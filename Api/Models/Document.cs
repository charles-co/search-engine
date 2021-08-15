// using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Document.Models 
{
    public record Doc {
        public Guid Id { get; init; }
        [Required]
        [Display(Name = "Supported Files .pdf | .xlx | .docx")]
        public string FilePath { get; set; }
        public DateTimeOffset AddedOn { get; init; }
    }
}