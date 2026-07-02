using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232.LAB_2_REST_API.Repositories.Entities
{
    public class AuditLog
    {
        [Key]
        public int AuditLogId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TableName { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        public string Action { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string PrimaryKey { get; set; } = null!;

        [Column(TypeName = "nvarchar(max)")]
        public string? OldValues { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string? NewValues { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public int? UserId { get; set; }
    }
}
