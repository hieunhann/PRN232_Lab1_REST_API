using System.Collections.Generic;

namespace PRN232.LAB_1_REST_API.API.Models
{
    public class CourseResponseModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
        public SemesterResponseModel? Semester { get; set; }
        
        public ICollection<EnrollmentResponseModel> Enrollments { get; set; } = new List<EnrollmentResponseModel>();
    }
}
