using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232.LAB_2_REST_API.Repositories.Entities
{
    public class Student : BaseEntity
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [Column(TypeName = "varchar(100)")]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Student code (e.g. SE19886). Optional.
        /// </summary>
        [MaxLength(20)]
        public string? StudentCode { get; set; }

        /// <summary>
        /// Student's phone number. Optional.
        /// </summary>
        [MaxLength(15)]
        [Column(TypeName = "varchar(15)")]
        public string? PhoneNumber { get; set; }

        // Navigation property
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
