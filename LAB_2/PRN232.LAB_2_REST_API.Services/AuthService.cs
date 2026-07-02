using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRN232.LAB_2_REST_API.Repositories;
using PRN232.LAB_2_REST_API.Repositories.Entities;
using PRN232.LAB_2_REST_API.Services.Interfaces;
using PRN232.LAB_2_REST_API.Services.Models.Requests;
using PRN232.LAB_2_REST_API.Services.Models.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PRN232.LAB_2_REST_API.Services
{
    /// <summary>
    /// Implement IAuthService: đăng nhập, tạo JWT, refresh token, BCrypt verify.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly LmsDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(LmsDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ─────────────────────────────────────────────────────────────────────
        // LOGIN
        // ─────────────────────────────────────────────────────────────────────
        public async Task<TokenResponse?> LoginAsync(LoginRequest request)
        {
            // 1. Tìm user theo username
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null) return null;

            // 2. Xác thực password bằng BCrypt
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return null;

            // 3. Tạo Access Token và Refresh Token
            var (accessToken, expiresIn) = GenerateAccessToken(user);
            var refreshToken = await GenerateRefreshTokenAsync(user.UserId);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = expiresIn,
                Role = user.Role,
                Username = user.Username
            };
        }

        // ─────────────────────────────────────────────────────────────────────
        // REFRESH TOKEN
        // ─────────────────────────────────────────────────────────────────────
        public async Task<TokenResponse?> RefreshTokenAsync(string refreshToken)
        {
            // 1. Tìm refresh token trong DB
            var tokenEntity = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);

            if (tokenEntity == null) return null;

            // 2. Kiểm tra token còn hiệu lực không
            if (tokenEntity.ExpiresAt < DateTime.UtcNow)
            {
                tokenEntity.IsRevoked = true;
                await _context.SaveChangesAsync();
                return null;
            }

            var user = tokenEntity.User;

            // 3. Thu hồi refresh token cũ
            tokenEntity.IsRevoked = true;
            await _context.SaveChangesAsync();

            // 4. Tạo cặp token mới
            var (accessToken, expiresIn) = GenerateAccessToken(user);
            var newRefreshToken = await GenerateRefreshTokenAsync(user.UserId);

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = expiresIn,
                Role = user.Role,
                Username = user.Username
            };
        }

        // ─────────────────────────────────────────────────────────────────────
        // REVOKE TOKEN
        // ─────────────────────────────────────────────────────────────────────
        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var tokenEntity = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);

            if (tokenEntity == null) return false;

            tokenEntity.IsRevoked = true;
            await _context.SaveChangesAsync();
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // REGISTER
        // ─────────────────────────────────────────────────────────────────────
        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return false;
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "User"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // CHANGE PASSWORD
        // ─────────────────────────────────────────────────────────────────────
        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        // ─────────────────────────────────────────────────────────────────────
        // PRIVATE HELPERS
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Tạo JWT Access Token với claims của user.
        /// </summary>
        private (string token, int expiresIn) GenerateAccessToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey chưa được cấu hình.");
            var issuer = jwtSettings["Issuer"] ?? "PRN232Lab2";
            var audience = jwtSettings["Audience"] ?? "PRN232Lab2Client";
            var expiresInMinutes = int.Parse(jwtSettings["ExpiresInMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiresInMinutes * 60);
        }

        /// <summary>
        /// Tạo Refresh Token ngẫu nhiên an toàn và lưu vào DB.
        /// </summary>
        private async Task<string> GenerateRefreshTokenAsync(int userId)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var refreshExpiryDays = int.Parse(jwtSettings["RefreshTokenExpiryDays"] ?? "7");

            // Tạo random token 64 bytes
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var token = Convert.ToBase64String(randomBytes);

            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddDays(refreshExpiryDays),
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return token;
        }
    }
}
