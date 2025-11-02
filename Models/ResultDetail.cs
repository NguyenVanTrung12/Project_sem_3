using System;
using System.Collections.Generic;

namespace Project_sem_3.Models
{
    public partial class ResultDetail
    {
        public int Id { get; set; }
        public int ResultId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public double? Mark { get; set; }

        public virtual Answer? Answer { get; set; }
        public virtual Question? Question { get; set; }
        public virtual Result? Result { get; set; }
    }
}
