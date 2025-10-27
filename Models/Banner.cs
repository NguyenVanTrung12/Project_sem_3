using System;
using System.Collections.Generic;

namespace Project_sem_3.Models
{
    public partial class Banner
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public int? Postion { get; set; }
        public int? Active { get; set; }
    }
}
