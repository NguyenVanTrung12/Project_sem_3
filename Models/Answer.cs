using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
namespace Project_sem_3.Models
{
    public partial class Answer
    {
        public Answer()
        {
            ResultDetails = new HashSet<ResultDetail>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Question is required.")]
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Answer content cannot be empty.")]
        [StringLength(500, ErrorMessage = "Answer content cannot exceed 500 characters.")]
        [Display(Name = "Answer Content")]
        public string? AnswerContent { get; set; }

        [Required(ErrorMessage = "Please specify whether this answer is correct or not.")]
        [Display(Name = "Correct Answer")]
        public bool? Correctly { get; set; }

        [Range(0, 1, ErrorMessage = "Status can only be 0 (hidden) or 1 (visible).")]
        public int? Status { get; set; }

        [JsonIgnore] // ❌ Ngăn serialize vòng lặp
        public virtual Question? Question { get; set; }
        public virtual ICollection<ResultDetail> ResultDetails { get; set; }
    }
}
