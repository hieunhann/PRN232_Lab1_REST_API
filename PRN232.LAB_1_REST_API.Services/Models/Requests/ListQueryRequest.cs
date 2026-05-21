using System.ComponentModel;

namespace PRN232.LAB_1_REST_API.Services.Models.Requests
{
    /// <summary>
    /// Các tham số dùng chung cho truy vấn danh sách.
    /// </summary>
    public class ListQueryRequest
    {
        /// <summary>
        /// Từ khóa tìm kiếm theo các trường chuỗi của entity.
        /// </summary>
        [Description("Từ khóa tìm kiếm theo các trường chuỗi của entity, ví dụ: tên, email, mã khóa học.")]
        public string? search { get; set; }

        /// <summary>
        /// Sắp xếp theo tên thuộc tính, có thể nhiều field phân tách bằng dấu phẩy.
        /// </summary>
        [Description("Sắp xếp theo tên thuộc tính, ví dụ: fullName hoặc -fullName để giảm dần. Có thể truyền nhiều field, phân tách bằng dấu phẩy.")]
        public string? sort { get; set; }

        /// <summary>
        /// Trang hiện tại, bắt đầu từ 1.
        /// </summary>
        [Description("Trang hiện tại, bắt đầu từ 1.")]
        public int page { get; set; } = 1;

        /// <summary>
        /// Số bản ghi trên mỗi trang.
        /// </summary>
        [Description("Số bản ghi trên mỗi trang. Nên chọn số dương, ví dụ 10, 20, 50.")]
        public int size { get; set; } = 10;

        /// <summary>
        /// Chỉ lấy các field cần thiết khi trả về dữ liệu.
        /// </summary>
        [Description("Danh sách field cần trả về, phân tách bằng dấu phẩy. Ví dụ: studentId,fullName,email")]
        public string? fields { get; set; }

        /// <summary>
        /// Nạp thêm navigation properties liên quan.
        /// </summary>
        [Description("Nạp thêm quan hệ liên quan, phân tách bằng dấu phẩy. Ví dụ: enrollments.course hoặc course.enrollments.student")]
        public string? expand { get; set; }

        /// <summary>
        /// Biểu thức lọc động cho danh sách.
        /// </summary>
        [Description("Biểu thức lọc động. Ví dụ: CourseId == 15, Status == \"Active\". Có thể kết hợp điều kiện với && hoặc ||.")]
        public string? filter { get; set; }
    }
}
