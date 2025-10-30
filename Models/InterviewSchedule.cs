using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project_sem_3.Models
{
    public partial class InterviewSchedule
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Candidate is required.")]
        public int CandidateId { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Interview Date")]
        public DateTime? InterviewDate { get; set; }

        [Required(ErrorMessage = "Interviewer name is required.")]
        [StringLength(100, ErrorMessage = "Interviewer name cannot exceed 100 characters.")]
        public string? Interviewer { get; set; }

        [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters.")]
        public string? Location { get; set; }

        [StringLength(100, ErrorMessage = "Result cannot exceed 100 characters.")]
        public string? Result { get; set; }

        [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters.")]
        public string? Note { get; set; }

        public virtual Candidate? Candidate { get; set; }
    }
}
