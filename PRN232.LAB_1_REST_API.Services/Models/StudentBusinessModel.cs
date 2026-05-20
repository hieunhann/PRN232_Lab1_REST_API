using System;
using System.Collections.Generic;

namespace PRN232.LAB_1_REST_API.Services.Models
{
    public class StudentBusinessModel
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        
        public ICollection<EnrollmentBusinessModel> Enrollments { get; set; } = new List<EnrollmentBusinessModel>();
    }
}
