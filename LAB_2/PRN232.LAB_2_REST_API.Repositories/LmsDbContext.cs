using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PRN232.LAB_2_REST_API.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;

namespace PRN232.LAB_2_REST_API.Repositories
{
    public class LmsDbContext : DbContext
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public LmsDbContext(DbContextOptions<LmsDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>Gets the currently authenticated user's ID from JWT claims.</summary>
        private int? CurrentUserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor?.HttpContext?.User
                    ?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(userIdClaim, out var id) ? id : (int?)null;
            }
        }

        public DbSet<Semester> Semesters { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries().ToList();
            var auditEntries = new List<AuditLog>();

            foreach (var entry in entries)
            {
                if (entry.Entity is BaseEntity baseEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        baseEntity.CreatedAt = DateTime.UtcNow;
                        baseEntity.CreatedBy = CurrentUserId;
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        baseEntity.UpdatedAt = DateTime.UtcNow;
                        baseEntity.UpdatedBy = CurrentUserId;
                    }
                    else if (entry.State == EntityState.Deleted)
                    {
                        // Soft delete
                        entry.State = EntityState.Modified;
                        baseEntity.IsDeleted = true;
                        baseEntity.UpdatedAt = DateTime.UtcNow;
                        baseEntity.UpdatedBy = CurrentUserId;
                    }
                }

                if (entry.Entity is not AuditLog && (entry.State == EntityState.Added || entry.State == EntityState.Modified || entry.State == EntityState.Deleted))
                {
                    var auditLog = new AuditLog
                    {
                        TableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name,
                        Action = entry.State.ToString(),
                        Timestamp = DateTime.UtcNow,
                        UserId = CurrentUserId
                    };

                    var primaryKeyProps = entry.Metadata.FindPrimaryKey()?.Properties;
                    if (primaryKeyProps != null && primaryKeyProps.Any())
                    {
                        var pkValues = primaryKeyProps.Select(p => entry.Property(p.Name).CurrentValue?.ToString() ?? "");
                        auditLog.PrimaryKey = string.Join(",", pkValues);
                    }
                    else
                    {
                        auditLog.PrimaryKey = "N/A";
                    }

                    if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
                    {
                        var oldVals = entry.OriginalValues.Properties.ToDictionary(p => p.Name, p => entry.OriginalValues[p]);
                        auditLog.OldValues = JsonSerializer.Serialize(oldVals);
                    }

                    if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                    {
                        var newVals = entry.CurrentValues.Properties.ToDictionary(p => p.Name, p => entry.CurrentValues[p]);
                        auditLog.NewValues = JsonSerializer.Serialize(newVals);
                    }

                    auditEntries.Add(auditLog);
                }
            }

            if (auditEntries.Any())
            {
                AuditLogs.AddRange(auditEntries);
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Relationships and Delete Behavior
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Semester)
                .WithMany(s => s.Courses)
                .HasForeignKey(c => c.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - RefreshToken relationship
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint on Username
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Soft Delete Query Filters
            modelBuilder.Entity<Student>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Course>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Enrollment>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Semester>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Subject>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);

            // Seed Data Generation
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // 5 Semesters
            var semesters = new List<Semester>();
            for (int i = 1; i <= 5; i++)
            {
                semesters.Add(new Semester
                {
                    SemesterId = i,
                    SemesterName = $"Semester {i} 2026",
                    StartDate = new DateTime(2026, 1, 1).AddMonths((i - 1) * 3),
                    EndDate = new DateTime(2026, 3, 30).AddMonths((i - 1) * 3)
                });
            }
            modelBuilder.Entity<Semester>().HasData(semesters);

            // 10 Subjects
            var subjects = new List<Subject>();
            for (int i = 1; i <= 10; i++)
            {
                subjects.Add(new Subject
                {
                    SubjectId = i,
                    SubjectCode = $"SUB{i:00}",
                    SubjectName = $"Subject {i} PRN232",
                    Credit = 3
                });
            }
            modelBuilder.Entity<Subject>().HasData(subjects);

            // 20 Courses
            var courses = new List<Course>();
            int courseId = 1;
            for (int s = 1; s <= 5; s++)
            {
                for (int c = 1; c <= 4; c++) // 5 * 4 = 20 courses
                {
                    courses.Add(new Course
                    {
                        CourseId = courseId,
                        CourseName = $"Course {courseId} for Sem {s}",
                        SemesterId = s
                    });
                    courseId++;
                }
            }
            modelBuilder.Entity<Course>().HasData(courses);

            // 50 Students
            var students = new List<Student>();
            for (int i = 1; i <= 50; i++)
            {
                students.Add(new Student
                {
                    StudentId = i,
                    FullName = $"Student Name {i}",
                    Email = $"student{i}@fpt.edu.vn",
                    DateOfBirth = new DateTime(2004, 1, 1).AddDays(i * 10)
                });
            }
            modelBuilder.Entity<Student>().HasData(students);

            // 500 Enrollments
            var enrollments = new List<Enrollment>();
            int enrollmentId = 1;
            Random random = new Random(123);
            for (int s = 1; s <= 50; s++) // 50 students
            {
                for (int e = 1; e <= 10; e++) // 10 courses per student
                {
                    enrollments.Add(new Enrollment
                    {
                        EnrollmentId = enrollmentId++,
                        StudentId = s,
                        CourseId = random.Next(1, 21), // Random course from 1 to 20
                        EnrollDate = new DateTime(2026, 1, 1).AddDays(random.Next(1, 100)),
                        Status = e % 2 == 0 ? "Completed" : "Active"
                    });
                }
            }
            modelBuilder.Entity<Enrollment>().HasData(enrollments);

            // Seed Users (password được hash bằng BCrypt, cost factor 11)
            // Admin: username=admin / password=123456
            // User:  username=user  / password=123456
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "admin",
                    PasswordHash = "$2b$11$H3BTc7xMNAlz5f0EsyUore7blBD8yq.i3qrj8ePFqGtGxI42M4CgS",
                    Role = "Admin"
                },
                new User
                {
                    UserId = 2,
                    Username = "user",
                    PasswordHash = "$2b$11$H3BTc7xMNAlz5f0EsyUore7blBD8yq.i3qrj8ePFqGtGxI42M4CgS",
                    Role = "User"
                }
            );
        }
    }
}
