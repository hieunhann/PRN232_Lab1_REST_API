using System;

namespace PRN232.LAB_2_REST_API.Services.Models
{
    public class EnrollmentBusinessModel
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = null!;
        public StudentBusinessModel? Student { get; set; }
        public CourseBusinessModel? Course { get; set; }
    }
}
