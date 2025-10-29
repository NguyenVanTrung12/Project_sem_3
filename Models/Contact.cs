using System;
using System.ComponentModel.DataAnnotations;

namespace Project_sem_3.Models
{
    public partial class Contact
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ và tên không được để trống.")]
        [StringLength(100, ErrorMessage = "Họ và tên không được vượt quá 100 ký tự.")]
        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        [StringLength(150, ErrorMessage = "Email không được vượt quá 150 ký tự.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Chủ đề không được để trống.")]
        [StringLength(200, ErrorMessage = "Chủ đề không được vượt quá 200 ký tự.")]
        [Display(Name = "Chủ đề")]
        public string? Subject { get; set; }

        [Required(ErrorMessage = "Nội dung tin nhắn không được để trống.")]
        [StringLength(1000, ErrorMessage = "Nội dung tin nhắn không được vượt quá 1000 ký tự.")]
        [Display(Name = "Tin nhắn")]
        public string? Message { get; set; }

        public int? Status { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedDate { get; set; }
    }
}
