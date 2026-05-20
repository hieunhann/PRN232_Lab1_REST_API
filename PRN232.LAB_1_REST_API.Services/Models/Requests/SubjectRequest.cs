namespace PRN232.LAB_1_REST_API.Services.Models.Requests
{
    public class SubjectRequest
    {
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credit { get; set; }
    }
}
