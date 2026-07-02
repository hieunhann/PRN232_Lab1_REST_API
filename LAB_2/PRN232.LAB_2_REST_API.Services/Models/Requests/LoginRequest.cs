using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB_2_REST_API.Services.Models.Requests
{
    /// <summary>
    /// Request model for Login API.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// User's username.
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; } = null!;

        /// <summary>
        /// User's password.
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
    }
}
