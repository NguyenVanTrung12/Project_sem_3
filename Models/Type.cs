using System;
using System.Collections.Generic;

namespace Project_sem_3.Models
{
    public partial class Type
    {
        public Type()
        {
            Questions = new HashSet<Question>();
            Results = new HashSet<Result>();
            SubjectTypes = new HashSet<SubjectType>();
        }

        public int Id { get; set; }
        public string? TypeName { get; set; }
        public int? Status { get; set; }
        public int? Ord { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Result> Results { get; set; }
        public virtual ICollection<SubjectType> SubjectTypes { get; set; }
    }
}
