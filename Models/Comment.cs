﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThienASPMVC08032023.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? CommentMsg { get; set; }

        public string? UserId { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public AppUser? User { get; set; }

        public int? ClipId { get; set; }

        [Required]
        [ForeignKey("ClipId")]
        public Clip? Clip { get; set; }

        public DateTime TimeCreated { get; set; } = DateTime.Now;
    }
}
