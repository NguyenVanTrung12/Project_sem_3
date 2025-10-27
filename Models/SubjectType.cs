using System;
using System.Collections.Generic;

namespace Project_sem_3.Models
{
    public partial class SubjectType
    {
        public int Id { get; set; }
        public int? SubjectId { get; set; }
        public int? TypeId { get; set; }
        public int? NumQuestion { get; set; }
        public int? LimitTime { get; set; }

        public virtual Subject? Subject { get; set; }
        public virtual Type? Type { get; set; }
    }
}
