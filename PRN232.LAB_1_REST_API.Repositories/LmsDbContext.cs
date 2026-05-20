using Microsoft.EntityFrameworkCore;
using PRN232.LAB_1_REST_API.Repositories.Entities;
using System;
using System.Collections.Generic;

namespace PRN232.LAB_1_REST_API.Repositories
{
    public class LmsDbContext : DbContext
    {
        public LmsDbContext(DbContextOptions<LmsDbContext> options) : base(options)
        {
        }

        public DbSet<Semester> Semesters { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Enrollment> Enrollments { get; set; } = null!;

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
        }
    }
}
