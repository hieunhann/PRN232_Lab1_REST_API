using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232.LAB_2_REST_API.Repositories.Entities
{
    public class Subject : BaseEntity
    {
        [Key]
        public int SubjectId { get; set; }

        [Required]
        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        public string SubjectCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string SubjectName { get; set; } = null!;

        public int Credit { get; set; }
    }
}
