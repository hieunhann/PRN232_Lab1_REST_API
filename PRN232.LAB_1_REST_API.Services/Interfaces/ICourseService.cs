using PRN232.LAB_1_REST_API.Services.Models;
using PRN232.LAB_1_REST_API.Services.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB_1_REST_API.Services.Interfaces
{
    public interface ICourseService
    {
        Task<CourseBusinessModel?> GetCourseByIdAsync(int id, string? expand);
        Task<(IEnumerable<CourseBusinessModel> Items, int TotalItems, int TotalPages)> GetCoursesAsync(string? search, string? sort, int page, int pageSize, string? expand, string? filter = null);
        Task<CourseBusinessModel> AddCourseAsync(CourseRequest request);
        Task<CourseBusinessModel?> UpdateCourseAsync(int id, CourseRequest request);
        Task<bool> DeleteCourseAsync(int id);

        /// <summary>
        /// Lấy danh sách sinh viên đã đăng ký (Enrollment) trong 1 khóa học theo CourseId
        /// </summary>
        Task<IEnumerable<StudentBusinessModel>> GetStudentsByCourseIdAsync(int courseId);
    }
}
