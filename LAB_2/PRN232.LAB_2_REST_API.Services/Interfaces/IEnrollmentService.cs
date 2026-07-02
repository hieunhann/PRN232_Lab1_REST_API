using PRN232.LAB_2_REST_API.Services.Models;
using PRN232.LAB_2_REST_API.Services.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB_2_REST_API.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentBusinessModel?> GetEnrollmentByIdAsync(int id, string? expand);
        Task<(IEnumerable<EnrollmentBusinessModel> Items, int TotalItems, int TotalPages)> GetEnrollmentsAsync(string? search, string? sort, int page, int pageSize, string? expand, string? filter = null);
        Task<EnrollmentBusinessModel> AddEnrollmentAsync(EnrollmentRequest request);
        Task<EnrollmentBusinessModel?> UpdateEnrollmentAsync(int id, EnrollmentRequest request);
        Task<bool> DeleteEnrollmentAsync(int id);
        
        /// <summary>
        /// Lấy danh sách tất cả sinh viên cùng học một lớp (khóa học) của một enrollment theo EnrollmentId
        /// </summary>
        Task<IEnumerable<StudentBusinessModel>?> GetStudentsByEnrollmentIdAsync(int enrollmentId);

        /// <summary>
        /// Lấy khóa học theo enrollment và kèm toàn bộ sinh viên đang tham gia khóa học đó
        /// </summary>
        Task<CourseBusinessModel?> GetCourseByEnrollmentIdAsync(int enrollmentId);
    }
}
