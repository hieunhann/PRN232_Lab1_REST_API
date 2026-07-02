using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232.LAB_2_REST_API.Repositories.Entities
{
    public class Course : BaseEntity
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CourseName { get; set; } = null!;

        public int SemesterId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(SemesterId))]
        public Semester Semester { get; set; } = null!;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
