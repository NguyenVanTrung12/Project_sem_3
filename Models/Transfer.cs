using System;
using System.Collections.Generic;

namespace Project_sem_3.Models
{
    public partial class Transfer
    {
        public int Id { get; set; }
        public int CandidateId { get; set; }
        public DateTime? TransferDate { get; set; }
        public string? FromStage { get; set; }
        public string? ToStage { get; set; }

        public virtual Candidate? Candidate { get; set; }
    }
}
