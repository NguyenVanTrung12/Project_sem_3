using System;
using System.Collections.Generic;

namespace Project_sem_3.Models
{
    public partial class Answer
    {
        public Answer()
        {
            ResultDetails = new HashSet<ResultDetail>();
        }

        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string? AnswerContent { get; set; }
        public bool? Correctly { get; set; }
        public int? Status { get; set; }

        public virtual Question Question { get; set; } = null!;
        public virtual ICollection<ResultDetail> ResultDetails { get; set; }
    }
}
