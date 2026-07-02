using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB_2_REST_API.Services.Models.Requests
{
    public class SemesterRequest
    {
        [Required(ErrorMessage = "SemesterName is required và không được để trống")]
        [StringLength(100, ErrorMessage = "SemesterName không được vượt quá 100 ký tự")]
        public string SemesterName { get; set; } = null!;

        [Required(ErrorMessage = "StartDate is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "EndDate is required")]
        public DateTime EndDate { get; set; }
    }
}
