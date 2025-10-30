using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Project_sem_3.Models
{
    public partial class online_aptitude_testsContext : DbContext
    {
        public online_aptitude_testsContext()
        {
        }

        public online_aptitude_testsContext(DbContextOptions<online_aptitude_testsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Answer> Answers { get; set; } = null!;
        public virtual DbSet<Banner> Banners { get; set; } = null!;
        public virtual DbSet<Blog> Blogs { get; set; } = null!;
        public virtual DbSet<BlogCategory> BlogCategories { get; set; } = null!;
        public virtual DbSet<Candidate> Candidates { get; set; } = null!;
        public virtual DbSet<Contact> Contacts { get; set; } = null!;
        public virtual DbSet<InterviewSchedule> InterviewSchedules { get; set; } = null!;
        public virtual DbSet<Manager> Managers { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<Result> Results { get; set; } = null!;
        public virtual DbSet<ResultDetail> ResultDetails { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<SubjectType> SubjectTypes { get; set; } = null!;
        public virtual DbSet<Transfer> Transfers { get; set; } = null!;
        public virtual DbSet<Type> Types { get; set; } = null!;

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Server=LAPTOP-MC0HPVFT;Database=online_aptitude_tests;uid=sa;pwd=1234#;MultipleActiveResultSets=True;");
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.Property(e => e.AnswerContent).HasColumnType("ntext");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Answers__Questio__2B3F6F97");
            });

            modelBuilder.Entity<Banner>(entity =>
            {
                entity.ToTable("Banner");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Blog>(entity =>
            {
                entity.HasIndex(e => e.Slug, "UQ__Blogs__BC7B5FB6A6EAF60A")
                    .IsUnique();

                entity.Property(e => e.Content).HasColumnType("ntext");

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Image).IsUnicode(false);

                entity.Property(e => e.Slug).HasMaxLength(255);

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.Tags).HasMaxLength(255);

                entity.Property(e => e.Title).HasMaxLength(255);

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

                entity.Property(e => e.Views).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.BlogCategory)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.BlogCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Blogs__BlogCateg__47DBAE45");

                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.ManagerId)
                    .HasConstraintName("FK__Blogs__ManagerId__48CFD27E");
            });

            modelBuilder.Entity<BlogCategory>(entity =>
            {
                entity.Property(e => e.CategoryName).HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
            });

            modelBuilder.Entity<Candidate>(entity =>
            {
                entity.HasIndex(e => e.CandidateCode, "UQ__Candidat__4A0AE081393ABB56")
                    .IsUnique();

                entity.Property(e => e.Address).HasMaxLength(255);

                entity.Property(e => e.BirthDate).HasColumnType("date");

                entity.Property(e => e.CandidateCode).HasMaxLength(255);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Education).HasMaxLength(255);

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.ExpectedPosition).HasMaxLength(100);

                entity.Property(e => e.Experience).HasColumnType("ntext");

                entity.Property(e => e.Faculty).HasMaxLength(255);

                entity.Property(e => e.Firstname).HasMaxLength(255);

                entity.Property(e => e.Fullname).HasMaxLength(255);

                entity.Property(e => e.Hometown).HasMaxLength(255);

                entity.Property(e => e.Lastname).HasMaxLength(255);

                entity.Property(e => e.Pass).HasMaxLength(100);

                entity.Property(e => e.Phone)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Semester).HasMaxLength(255);

                entity.Property(e => e.Skills).HasMaxLength(255);

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.University).HasMaxLength(255);

                entity.HasOne(d => d.Manager)
                    .WithMany(p => p.Candidates)
                    .HasForeignKey(d => d.ManagerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Candidate__Manag__1BFD2C07");
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.ToTable("Contact");

                entity.Property(e => e.CreatedDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.FullName).HasMaxLength(255);

                entity.Property(e => e.Message).HasColumnType("ntext");

                entity.Property(e => e.Status).HasDefaultValueSql("((0))");

                entity.Property(e => e.Subject).HasMaxLength(255);
            });

            modelBuilder.Entity<InterviewSchedule>(entity =>
            {
                entity.Property(e => e.InterviewDate).HasColumnType("datetime");

                entity.Property(e => e.Interviewer).HasMaxLength(255);

                entity.Property(e => e.Location).HasMaxLength(255);

                entity.Property(e => e.Note).HasMaxLength(255);

                entity.Property(e => e.Result).HasMaxLength(50);

                entity.HasOne(d => d.Candidate)
                    .WithMany(p => p.InterviewSchedules)
                    .HasForeignKey(d => d.CandidateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Interview__Candi__3D5E1FD2");
            });

            modelBuilder.Entity<Manager>(entity =>
            {
                entity.HasIndex(e => e.Username, "UQ__Managers__536C85E46EFC9153")
                    .IsUnique();

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Fullname).HasMaxLength(255);

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.Username)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Managers)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Managers__RoleId__164452B1");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.Property(e => e.QuestionContent).HasColumnType("ntext");

                entity.Property(e => e.QuestionTitle).HasColumnType("ntext");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Questions__Subje__286302EC");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Questions__TypeI__276EDEB3");
            });

            modelBuilder.Entity<Result>(entity =>
            {
                entity.Property(e => e.SubmitDate).HasColumnType("datetime");

                entity.HasOne(d => d.Candidate)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.CandidateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Results__Candida__2E1BDC42");

                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.SubjectId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Results__Subject__2F10007B");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Results)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Results__TypeId__300424B4");
            });

            modelBuilder.Entity<ResultDetail>(entity =>
            {
                entity.HasOne(d => d.Answer)
                    .WithMany(p => p.ResultDetails)
                    .HasForeignKey(d => d.AnswerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ResultDet__Answe__34C8D9D1");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.ResultDetails)
                    .HasForeignKey(d => d.QuestionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ResultDet__Quest__33D4B598");

                entity.HasOne(d => d.Result)
                    .WithMany(p => p.ResultDetails)
                    .HasForeignKey(d => d.ResultId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ResultDet__Resul__32E0915F");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160BA573BF9")
                    .IsUnique();

                entity.Property(e => e.RoleName).HasMaxLength(255);
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.Property(e => e.Status).HasDefaultValueSql("((1))");

                entity.Property(e => e.SubjectCode).HasMaxLength(255);

                entity.Property(e => e.SubjectName).HasMaxLength(255);
            });

            modelBuilder.Entity<SubjectType>(entity =>
            {
                entity.HasOne(d => d.Subject)
                    .WithMany(p => p.SubjectTypes)
                    .HasForeignKey(d => d.SubjectId)
                    .HasConstraintName("FK__SubjectTy__Subje__239E4DCF");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.SubjectTypes)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK__SubjectTy__TypeI__24927208");
            });

            modelBuilder.Entity<Transfer>(entity =>
            {
                entity.Property(e => e.FromStage)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("('Test Round')");

                entity.Property(e => e.ToStage)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("('HR Round')");

                entity.Property(e => e.TransferDate)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Candidate)
                    .WithMany(p => p.Transfers)
                    .HasForeignKey(d => d.CandidateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Transfers__Candi__3A81B327");
            });

            modelBuilder.Entity<Type>(entity =>
            {
                entity.Property(e => e.TypeName).HasMaxLength(255);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
