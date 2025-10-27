using System;
using System.Collections.Generic;

namespace Project_sem_3.Models
{
    public partial class Role
    {
        public Role()
        {
            Managers = new HashSet<Manager>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; } = null!;

        public virtual ICollection<Manager> Managers { get; set; }
    }
}
