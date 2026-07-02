using PRN232.LAB_2_REST_API.Services.Models;
using PRN232.LAB_2_REST_API.Services.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB_2_REST_API.Services.Interfaces
{
    public interface ISemesterService
    {
        Task<SemesterBusinessModel?> GetSemesterByIdAsync(int id, string? expand);
        Task<(IEnumerable<SemesterBusinessModel> Items, int TotalItems, int TotalPages)> GetSemestersAsync(string? search, string? sort, int page, int pageSize, string? expand, string? filter = null);
        Task<SemesterBusinessModel> AddSemesterAsync(SemesterRequest request);
        Task<SemesterBusinessModel?> UpdateSemesterAsync(int id, SemesterRequest request);
        Task<bool> DeleteSemesterAsync(int id);

        /// <summary>
        /// Lấy danh sách tất cả các khóa học thuộc học kỳ tương ứng theo SemesterId
        /// </summary>
        Task<IEnumerable<CourseBusinessModel>?> GetCoursesBySemesterIdAsync(int semesterId);
    }
}
