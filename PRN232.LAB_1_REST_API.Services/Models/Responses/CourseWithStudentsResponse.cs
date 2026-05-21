using System.Collections.Generic;

namespace PRN232.LAB_1_REST_API.Services.Models.Responses
{
    public class CourseWithStudentsResponse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
        public SemesterResponse? Semester { get; set; }
        public ICollection<StudentSummaryResponse> Students { get; set; } = new List<StudentSummaryResponse>();
    }
}