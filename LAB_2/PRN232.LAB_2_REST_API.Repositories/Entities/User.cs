using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB_2_REST_API.Repositories.Entities
{
    /// <summary>
    /// Represents a user account in the LMS system.
    /// </summary>
    public class User : BaseEntity
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = null!;

        /// <summary>
        /// Role: "Admin" or "User"
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "User";

        // Navigation
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
