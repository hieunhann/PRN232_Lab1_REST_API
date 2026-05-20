using System;
using System.Collections.Generic;

namespace PRN232.LAB_1_REST_API.API.Models
{
    public class StudentResponseModel
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        
        public ICollection<EnrollmentResponseModel> Enrollments { get; set; } = new List<EnrollmentResponseModel>();
    }
}
