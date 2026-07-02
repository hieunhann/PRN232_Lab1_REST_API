using PRN232.LAB_2_REST_API.Services.Models;
using PRN232.LAB_2_REST_API.Services.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB_2_REST_API.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<SubjectBusinessModel?> GetSubjectByIdAsync(int id, string? expand);
        Task<(IEnumerable<SubjectBusinessModel> Items, int TotalItems, int TotalPages)> GetSubjectsAsync(string? search, string? sort, int page, int pageSize, string? expand, string? filter = null);
        Task<SubjectBusinessModel> AddSubjectAsync(SubjectRequest request);
        Task<SubjectBusinessModel?> UpdateSubjectAsync(int id, SubjectRequest request);
        Task<bool> DeleteSubjectAsync(int id);
    }
}
