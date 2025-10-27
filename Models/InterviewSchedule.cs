using System;
using System.Collections.Generic;

namespace Project_sem_3.Models
{
    public partial class InterviewSchedule
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public DateTime InterviewDate { get; set; }
        public string? Interviewer { get; set; }
        public string? Location { get; set; }
        public string? Result { get; set; }
        public string? Note { get; set; }

        public virtual Candidate Candidate { get; set; } = null!;
    }
}
