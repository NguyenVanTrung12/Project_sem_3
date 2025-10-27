using System;
using System.Collections.Generic;

namespace Project_sem_3.Models
{
    public partial class Blog
    {
        public int Id { get; set; }
        public int BlogCategoryId { get; set; }
        public int? ManagerId { get; set; }
        public string Title { get; set; } = null!;
        public string? Slug { get; set; }
        public string? Content { get; set; }
        public string? Image { get; set; }
        public string? Tags { get; set; }
        public int? Views { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual BlogCategory BlogCategory { get; set; } = null!;
        public virtual Manager? Manager { get; set; }
    }
}
