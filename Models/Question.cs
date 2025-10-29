using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [Required(ErrorMessage = "Question type is required.")]
        public int TypeId { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        public int SubjectId { get; set; }

        [Required(ErrorMessage = "Question title is required.")]
        [StringLength(255, ErrorMessage = "Question title cannot exceed 255 characters.")]
        public string? QuestionTitle { get; set; }

        [Required(ErrorMessage = "Question content is required.")]
        [StringLength(2000, ErrorMessage = "Question content cannot exceed 2000 characters.")]
        public string? QuestionContent { get; set; }

        [Required(ErrorMessage = "Mark is required.")]
        [Range(0.1, 100, ErrorMessage = "Mark must be between 0.1 and 100.")]
        public double? Mark { get; set; }

        [Range(0, 1, ErrorMessage = "Status must be 0 (inactive) or 1 (active).")]
        public int? Status { get; set; }

        public virtual Subject Subject { get; set; } = null!;
        public virtual Type Type { get; set; } = null!;
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<ResultDetail> ResultDetails { get; set; }
    }
}
