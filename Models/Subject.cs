using System;
using System.Collections.Generic;

namespace Project_sem_3.Models
{
    public partial class Subject
    {
        public Subject()
        {
            Questions = new HashSet<Question>();
            Results = new HashSet<Result>();
            SubjectTypes = new HashSet<SubjectType>();
        }

        public int Id { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public int? LimitTime { get; set; }
        public int? QuestionCount { get; set; }
        public int? OrderIndex { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Result> Results { get; set; }
        public virtual ICollection<SubjectType> SubjectTypes { get; set; }
    }
}
