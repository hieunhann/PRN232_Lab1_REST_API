using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB_2_REST_API.Services.Models.Requests
{
    public class CourseRequest
    {
        [Required(ErrorMessage = "CourseName is required và không được để trống")]
        [StringLength(100, ErrorMessage = "CourseName không được vượt quá 100 ký tự")]
        public string CourseName { get; set; } = null!;

        [Required(ErrorMessage = "SemesterId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "SemesterId phải lớn hơn 0")]
        public int SemesterId { get; set; }
    }
}
