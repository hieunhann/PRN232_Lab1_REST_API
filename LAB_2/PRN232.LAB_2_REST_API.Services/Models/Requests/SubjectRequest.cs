using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB_2_REST_API.Services.Models.Requests
{
    public class SubjectRequest
    {
        [Required(ErrorMessage = "SubjectCode is required và không được để trống")]
        [StringLength(20, ErrorMessage = "SubjectCode không được vượt quá 20 ký tự")]
        public string SubjectCode { get; set; } = null!;

        [Required(ErrorMessage = "SubjectName is required và không được để trống")]
        [StringLength(100, ErrorMessage = "SubjectName không được vượt quá 100 ký tự")]
        public string SubjectName { get; set; } = null!;

        [Required(ErrorMessage = "Credit is required")]
        [Range(1, 10, ErrorMessage = "Credit phải nằm trong khoảng từ 1 đến 10")]
        public int Credit { get; set; }
    }
}
