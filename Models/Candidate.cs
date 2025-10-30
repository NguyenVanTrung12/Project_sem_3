using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project_sem_3.Models
{
    public partial class Candidate
    {
        public Candidate()
        {
            InterviewSchedules = new HashSet<InterviewSchedule>();
            Results = new HashSet<Result>();
            Transfers = new HashSet<Transfer>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Manager is required.")]
        public int ManagerId { get; set; }

        [StringLength(50, ErrorMessage = "Candidate code cannot exceed 50 characters.")]
        public string? CandidateCode { get; set; }
        public string? Pass { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string? Firstname { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string? Lastname { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string? Fullname { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public bool? Gender { get; set; }

        [Required(ErrorMessage = "Birth date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date.")]
        public DateTime? BirthDate { get; set; }

        [StringLength(100, ErrorMessage = "Hometown cannot exceed 100 characters.")]
        public string? Hometown { get; set; }

        [StringLength(100, ErrorMessage = "University name cannot exceed 100 characters.")]
        public string? University { get; set; }

        [StringLength(50, ErrorMessage = "Semester cannot exceed 50 characters.")]
        public string? Semester { get; set; }

        [StringLength(100, ErrorMessage = "Faculty cannot exceed 100 characters.")]
        public string? Faculty { get; set; }

        [StringLength(100, ErrorMessage = "Education cannot exceed 100 characters.")]
        public string? Education { get; set; }

        [StringLength(255, ErrorMessage = "Experience cannot exceed 255 characters.")]
        public string? Experience { get; set; }

        [StringLength(255, ErrorMessage = "Skills cannot exceed 255 characters.")]
        public string? Skills { get; set; }

        [StringLength(100, ErrorMessage = "Expected position cannot exceed 100 characters.")]
        public string? ExpectedPosition { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string? Phone { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string? Email { get; set; }

        [Range(0, 1, ErrorMessage = "Status must be 0 (inactive) or 1 (active).")]
        public int? Status { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date.")]
        public DateTime? CreatedAt { get; set; }

        public virtual Manager? Manager { get; set; }
        public virtual ICollection<InterviewSchedule> InterviewSchedules { get; set; }
        public virtual ICollection<Result> Results { get; set; }
        public virtual ICollection<Transfer> Transfers { get; set; }
    }
}
