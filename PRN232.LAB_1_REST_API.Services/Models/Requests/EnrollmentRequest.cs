using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB_1_REST_API.Services.Models.Requests
{
    public class EnrollmentRequest
    {
        [Required(ErrorMessage = "StudentId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "StudentId phải lớn hơn 0")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "CourseId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "CourseId phải lớn hơn 0")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "EnrollDate is required")]
        public DateTime EnrollDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Active|Completed|Dropped)$", ErrorMessage = "Status phải là một trong các giá trị: Active, Completed, Dropped")]
        [StringLength(20, ErrorMessage = "Status không được vượt quá 20 ký tự")]
        public string Status { get; set; } = "Active";
    }
}
