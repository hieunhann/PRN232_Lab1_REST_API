using System;

namespace PRN232.LAB_1_REST_API.API.Models
{
    public class EnrollmentResponseModel
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = null!;
        public StudentResponseModel? Student { get; set; }
        public CourseResponseModel? Course { get; set; }
    }
}
