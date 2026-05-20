namespace PRN232.LAB_1_REST_API.Services.Models.Requests
{
    public class CourseRequest
    {
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
    }
}
