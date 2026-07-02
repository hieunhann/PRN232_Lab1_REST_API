using System;
using System.Collections.Generic;

namespace PRN232.LAB_2_REST_API.Services.Models
{
    public class StudentBusinessModel
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }

        /// <summary>Student code at FPTU (e.g. SE19886). V2 only.</summary>
        public string? StudentCode { get; set; }

        /// <summary>Phone number. V2 only.</summary>
        public string? PhoneNumber { get; set; }

        public ICollection<EnrollmentBusinessModel> Enrollments { get; set; } = new List<EnrollmentBusinessModel>();
    }
}
