using System;

namespace PRN232.LAB_1_REST_API.Services.Models.Requests
{
    public class EnrollmentRequest
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = "Active";
    }
}
