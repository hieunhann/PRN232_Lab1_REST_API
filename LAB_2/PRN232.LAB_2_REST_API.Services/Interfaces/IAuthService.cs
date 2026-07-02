using PRN232.LAB_2_REST_API.Services.Models.Requests;
using PRN232.LAB_2_REST_API.Services.Models.Responses;

namespace PRN232.LAB_2_REST_API.Services.Interfaces
{
    /// <summary>
    /// Interface cho Authentication Service - Xử lý đăng nhập và refresh token.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Đăng nhập bằng username và password, trả về JWT token nếu hợp lệ.
        /// </summary>
        Task<TokenResponse?> LoginAsync(LoginRequest request);

        /// <summary>
        /// Cấp lại Access Token mới bằng Refresh Token còn hiệu lực.
        /// </summary>
        Task<TokenResponse?> RefreshTokenAsync(string refreshToken);

        /// <summary>
        /// Revoke Refresh Token (logout).
        /// </summary>
        Task<bool> RevokeTokenAsync(string refreshToken);

        /// <summary>
        /// Register a new user account.
        /// </summary>
        Task<bool> RegisterAsync(RegisterRequest request);

        /// <summary>
        /// Change the password for the current user.
        /// </summary>
        Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    }
}
