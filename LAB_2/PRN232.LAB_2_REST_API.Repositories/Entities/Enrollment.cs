using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232.LAB_2_REST_API.Repositories.Entities
{
    public class Enrollment : BaseEntity
    {
        [Key]
        public int EnrollmentId { get; set; }

        public int StudentId { get; set; }

        public int CourseId { get; set; }

        public DateTime EnrollDate { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        public string Status { get; set; } = null!;

        // Navigation properties
        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; } = null!;

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; } = null!;
    }
}
