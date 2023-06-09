﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThienASPMVC08032023.Models
{
    public class Clip
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Clip title")]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public string? Url { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
        
        public string? AuthorId { get; set; }

        [ForeignKey("AuthorId")]
        public AppUser? AuthorUser { get; set; }

        public List<MainComment>? MainComments { get; set; }

        public DateTime TimeCreated { get; set; } = DateTime.Now;   
    }
}
