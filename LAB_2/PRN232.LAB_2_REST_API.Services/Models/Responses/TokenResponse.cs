namespace PRN232.LAB_2_REST_API.Services.Models.Responses
{
    /// <summary>
    /// Response model sau khi đăng nhập hoặc refresh token thành công.
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// JWT Access Token để gọi các API được bảo vệ.
        /// </summary>
        public string AccessToken { get; set; } = null!;

        /// <summary>
        /// Refresh Token để cấp lại Access Token khi hết hạn.
        /// </summary>
        public string RefreshToken { get; set; } = null!;

        /// <summary>
        /// Thời gian tồn tại của Access Token (tính bằng giây).
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Role của người dùng.
        /// </summary>
        public string Role { get; set; } = null!;

        /// <summary>
        /// Tên đăng nhập.
        /// </summary>
        public string Username { get; set; } = null!;
    }
}
