using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LAB_2_REST_API.Services.Interfaces;
using PRN232.LAB_2_REST_API.Services.Models.Requests;
using PRN232.LAB_2_REST_API.Services.Models.Responses;

namespace PRN232.LAB_2_REST_API.API.Controllers
{
    /// <summary>
    /// Auth Controller - Handles login, token refresh, revocation, registration, and password changes.
    /// </summary>
    [Route("api/auth")]
    [Route("api/v{version:apiVersion}/auth")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json", "application/xml")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Login and receive JWT Access Token + Refresh Token.
        /// </summary>
        /// <param name="request">Login information (username, password)</param>
        /// <returns>Token response containing accessToken, refreshToken, and expiration time</returns>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid login data.",
                    Errors = ModelState
                });
            }

            var tokenResponse = await _authService.LoginAsync(request);

            if (tokenResponse == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Incorrect username or password.",
                    Errors = "Unauthorized"
                });
            }

            return Ok(new ApiResponse<TokenResponse>
            {
                Success = true,
                Message = "Login successful!",
                Data = tokenResponse
            });
        }

        /// <summary>
        /// Refresh Access Token using valid Refresh Token.
        /// </summary>
        /// <param name="request">Refresh Token</param>
        /// <returns>New token pair</returns>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid RefreshToken.",
                    Errors = ModelState
                });
            }

            var tokenResponse = await _authService.RefreshTokenAsync(request.RefreshToken);

            if (tokenResponse == null)
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Refresh Token has expired or is invalid. Please log in again.",
                    Errors = "Unauthorized"
                });
            }

            return Ok(new ApiResponse<TokenResponse>
            {
                Success = true,
                Message = "Token refreshed successfully!",
                Data = tokenResponse
            });
        }

        /// <summary>
        /// Revoke Refresh Token (logout from current device).
        /// </summary>
        /// <param name="request">Refresh Token to revoke</param>
        [HttpPost("revoke-token")]
        [Authorize]
        public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _authService.RevokeTokenAsync(request.RefreshToken);

            if (!result)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Refresh Token does not exist or has been revoked.",
                    Errors = "404 Not Found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Logged out successfully."
            });
        }
        /// <summary>
        /// Register a new user account.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid registration data.",
                    Errors = ModelState
                });
            }

            var result = await _authService.RegisterAsync(request);
            if (!result)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Username already exists.",
                    Errors = "Registration Failed"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Registration successful. You can now log in."
            });
        }

        /// <summary>
        /// Change password for the authenticated user.
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid data.",
                    Errors = ModelState
                });
            }

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = "User not authenticated properly.",
                    Errors = "Unauthorized"
                });
            }

            var result = await _authService.ChangePasswordAsync(userId, request);
            if (!result)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Current password is incorrect.",
                    Errors = "Password Change Failed"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Password changed successfully."
            });
        }
    }
}
