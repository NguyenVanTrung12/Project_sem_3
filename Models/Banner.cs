using System;
using System.ComponentModel.DataAnnotations;

namespace Project_sem_3.Models
{
    public partial class Banner
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Banner name is required.")]
        [StringLength(200, ErrorMessage = "Banner name cannot exceed 200 characters.")]
        [Display(Name = "Banner Name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Please select a banner image.")]
        [StringLength(255, ErrorMessage = "Image file name cannot exceed 255 characters.")]
        [Display(Name = "Banner Image")]
        public string? Image { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Display(Name = "Display Position")]
        public int? Postion { get; set; }

        [Display(Name = "Active")]
        public int? Active { get; set; }
    }
}
