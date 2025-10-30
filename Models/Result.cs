using System;
using System.Collections.Generic;

namespace Project_sem_3.Models
{
    public partial class Result
    {
        public Result()
        {
            ResultDetails = new HashSet<ResultDetail>();
        }

        public int Id { get; set; }
        public int CandidateId { get; set; }
        public int SubjectId { get; set; }
        public int TypeId { get; set; }
        public double? TotalMark { get; set; }
        public DateTime? SubmitDate { get; set; }
        public int? Status { get; set; }

        public virtual Candidate? Candidate { get; set; }
        public virtual Subject? Subject { get; set; }
        public virtual Type? Type { get; set; } 
        public virtual ICollection<ResultDetail> ResultDetails { get; set; }
    }
}
