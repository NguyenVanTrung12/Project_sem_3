using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

    
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; } = null!;

     
        [StringLength(255, ErrorMessage = "Password cannot exceed 255 characters.")]
        public string? PasswordHash { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string Fullname { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string? Email { get; set; }

        public string? Image { get; set; }

        [Required(ErrorMessage = "Phone is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public int? RoleId { get; set; }

        [Range(0, 1, ErrorMessage = "Status must be 0 (inactive) or 1 (active).")]
        public int? Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CreatedAt { get; set; }

        public virtual Role? Role { get; set; }
        public virtual ICollection<Blog> Blogs { get; set; }
        public virtual ICollection<Candidate> Candidates { get; set; }
    }
}
