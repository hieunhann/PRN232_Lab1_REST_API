namespace PRN232.LAB_2_REST_API.Services.Models
{
    public class SubjectBusinessModel
    {
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credit { get; set; }
    }
}
