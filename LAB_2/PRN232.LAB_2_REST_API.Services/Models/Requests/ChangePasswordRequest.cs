using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB_2_REST_API.Services.Models.Requests
{
    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "New password must be at least 6 characters long")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$", ErrorMessage = "New password must contain at least 1 uppercase letter, 1 number, and 1 special character")]
        public string NewPassword { get; set; } = null!;
    }
}
