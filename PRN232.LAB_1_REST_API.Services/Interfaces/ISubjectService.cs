using PRN232.LAB_1_REST_API.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB_1_REST_API.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<SubjectBusinessModel?> GetSubjectByIdAsync(int id, string? expand);
        Task<(IEnumerable<SubjectBusinessModel> Items, int TotalItems, int TotalPages)> GetSubjectsAsync(string? search, string? sort, int page, int pageSize, string? expand, string? filter = null);
    }
}
