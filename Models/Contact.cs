using System;
using System.ComponentModel.DataAnnotations;

namespace Project_sem_3.Models
{
    public partial class Contact
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters.")]
        [Display(Name = "Subject")]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Message content cannot be empty.")]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters.")]
        [Display(Name = "Message")]
        public string? Message { get; set; }

        public int? Status { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Created Date")]
        public DateTime? CreatedDate { get; set; }
    }
}
