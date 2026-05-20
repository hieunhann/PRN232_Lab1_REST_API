namespace PRN232.LAB_1_REST_API.API.Models
{
    public class SubjectResponseModel
    {
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credit { get; set; }
    }
}
