using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB_2_REST_API.Services.Models.Requests
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$", ErrorMessage = "Password must contain at least 1 uppercase letter, 1 number, and 1 special character")]
        public string Password { get; set; } = null!;
    }
}
