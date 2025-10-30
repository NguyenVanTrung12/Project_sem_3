using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project_sem_3.Models
{
    public partial class Blog
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Blog category is required.")]
        public int? BlogCategoryId { get; set; }

        [Required(ErrorMessage = "Manager is required.")]
        public int? ManagerId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string? Title { get; set; }

        [StringLength(200, ErrorMessage = "Slug cannot exceed 200 characters.")]
        public string? Slug { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string? Content { get; set; }

        public string? Image { get; set; }
        public string? Tags { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Invalid number of views.")]
        public int? Views { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual BlogCategory? BlogCategory { get; set; }
        public virtual Manager? Manager { get; set; }
    }
}
