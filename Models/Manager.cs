using System;
using System.Collections.Generic;

namespace Project_sem_3.Models
{
    public partial class Manager
    {
        public Manager()
        {
            Blogs = new HashSet<Blog>();
            Candidates = new HashSet<Candidate>();
        }

        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? PasswordHash { get; set; }
        public string Fullname { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public int? RoleId { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public virtual Role? Role { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<Candidate> Candidates { get; set; }
    }
}
