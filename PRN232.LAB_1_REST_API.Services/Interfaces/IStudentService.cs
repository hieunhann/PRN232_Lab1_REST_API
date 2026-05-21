using PRN232.LAB_1_REST_API.Services.Models;
using PRN232.LAB_1_REST_API.Services.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB_1_REST_API.Services.Interfaces
{
    public interface IStudentService
    {
        Task<StudentBusinessModel?> GetStudentByIdAsync(int id, string? expand);
        Task<(IEnumerable<StudentBusinessModel> Items, int TotalItems, int TotalPages)> GetStudentsAsync(string? search, string? sort, int page, int pageSize, string? expand, string? filter = null);

        /// <summary>
        /// Tạo mới một học sinh vào hệ thống cơ sở dữ liệu
        /// </summary>
        /// <param name="request">Thông tin sinh viên cần thêm từ client</param>
        /// <returns>Dữ liệu sinh viên vừa tạo sau khi được lưu thành công kèm theo ID tự tăng</returns>
        Task<StudentBusinessModel> AddStudentAsync(StudentRequest request);

        /// <summary>
        /// Cập nhật thông tin học sinh
        /// </summary>
        Task<StudentBusinessModel?> UpdateStudentAsync(int id, StudentRequest request);

        /// <summary>
        /// Xóa học sinh khỏi hệ thống
        /// </summary>
        Task<bool> DeleteStudentAsync(int id);

        /// <summary>
        /// Lấy danh sách tất cả các khóa học mà sinh viên đã đăng ký theo StudentId
        /// </summary>
        Task<IEnumerable<CourseBusinessModel>?> GetCoursesByStudentIdAsync(int studentId);
    }
}
