using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project_sem_3.Models
{
    public partial class Answer
    {
        public Answer()
        {
            ResultDetails = new HashSet<ResultDetail>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Câu hỏi là bắt buộc.")]
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Nội dung câu trả lời không được để trống.")]
        [StringLength(500, ErrorMessage = "Nội dung câu trả lời không được vượt quá 500 ký tự.")]
        [Display(Name = "Nội dung câu trả lời")]
        public string? AnswerContent { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn đáp án đúng hoặc sai.")]
        [Display(Name = "Đáp án đúng")]
        public bool? Correctly { get; set; }

        [Range(0, 1, ErrorMessage = "Trạng thái chỉ có thể là 0 (ẩn) hoặc 1 (hiển thị).")]
        public int? Status { get; set; }

        public virtual Question Question { get; set; } = null!;
        public virtual ICollection<ResultDetail> ResultDetails { get; set; }
    }
}
