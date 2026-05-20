using System;
using System.Collections.Generic;

namespace PRN232.LAB_1_REST_API.Services.Models.Responses
{
    public class StudentResponse
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        
        public ICollection<EnrollmentResponse> Enrollments { get; set; } = new List<EnrollmentResponse>();
    }
}
