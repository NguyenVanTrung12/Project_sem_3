using System;
using System.Collections.Generic;

namespace Project_sem_3.Models
{
    public partial class Question
    {
        public Question()
        {
            Answers = new HashSet<Answer>();
            ResultDetails = new HashSet<ResultDetail>();
        }

        public int Id { get; set; }
        public int TypeId { get; set; }
        public int SubjectId { get; set; }
        public string? QuestionTitle { get; set; }
        public string? QuestionContent { get; set; }
        public double? Mark { get; set; }
        public int? Status { get; set; }

        public virtual Subject Subject { get; set; } = null!;
        public virtual Type Type { get; set; } = null!;
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<ResultDetail> ResultDetails { get; set; }
    }
}
