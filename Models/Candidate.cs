using System;
using System.Collections.Generic;

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
        public int ManagerId { get; set; }
        public string? CandidateCode { get; set; }
        public string? Pass { get; set; }
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Fullname { get; set; }
        public bool? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Hometown { get; set; }
        public string? University { get; set; }
        public string? Semester { get; set; }
        public string? Faculty { get; set; }
        public string? Education { get; set; }
        public string? Experience { get; set; }
        public string? Skills { get; set; }
        public string? ExpectedPosition { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public int? Status { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Manager Manager { get; set; } = null!;
        public virtual ICollection<InterviewSchedule> InterviewSchedules { get; set; }
        public virtual ICollection<Result> Results { get; set; }
        public virtual ICollection<Transfer> Transfers { get; set; }
    }
}
