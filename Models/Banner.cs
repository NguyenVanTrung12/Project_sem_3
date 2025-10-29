using System;
using System.ComponentModel.DataAnnotations;

namespace Project_sem_3.Models
{
    public partial class Banner
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên banner không được để trống.")]
        [StringLength(200, ErrorMessage = "Tên banner không được vượt quá 200 ký tự.")]
        [Display(Name = "Tên banner")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ảnh banner.")]
        [StringLength(255, ErrorMessage = "Tên tệp ảnh không được vượt quá 255 ký tự.")]
        [Display(Name = "Ảnh banner")]
        public string? Image { get; set; }

        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        
        [Display(Name = "Vị trí hiển thị")]
        public int? Postion { get; set; }

        
        [Display(Name = "Kích hoạt")]
        public int? Active { get; set; }
    }
}
