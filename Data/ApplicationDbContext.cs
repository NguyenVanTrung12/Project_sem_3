using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project_sem_3.Models;

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project_sem_3.Models.Subject> Subject { get; set; } = default!;

public DbSet<Project_sem_3.Models.SubjectType> SubjectType { get; set; } = default!;

public DbSet<Project_sem_3.Models.Type> Type { get; set; } = default!;
    }
