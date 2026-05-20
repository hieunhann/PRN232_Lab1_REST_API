using System.Collections.Generic;

namespace PRN232.LAB_1_REST_API.Services.Models
{
    public class CourseBusinessModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
        public SemesterBusinessModel? Semester { get; set; }
        
        public ICollection<EnrollmentBusinessModel> Enrollments { get; set; } = new List<EnrollmentBusinessModel>();
    }
}
