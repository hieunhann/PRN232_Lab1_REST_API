using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232.LAB_2_REST_API.Repositories.Entities
{
    public class Semester : BaseEntity
    {
        [Key]
        public int SemesterId { get; set; }

        [Required]
        [MaxLength(100)]
        public string SemesterName { get; set; } = null!;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        // Navigation property
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
