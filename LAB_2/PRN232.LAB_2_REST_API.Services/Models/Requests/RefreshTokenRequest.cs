using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB_2_REST_API.Services.Models.Requests
{
    /// <summary>
    /// Request model để làm mới Access Token.
    /// </summary>
    public class RefreshTokenRequest
    {
        /// <summary>
        /// Refresh Token hợp lệ nhận được từ Login.
        /// </summary>
        [Required(ErrorMessage = "RefreshToken không được để trống")]
        public string RefreshToken { get; set; } = null!;
    }
}
