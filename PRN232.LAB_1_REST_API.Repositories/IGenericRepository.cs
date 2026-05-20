using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB_1_REST_API.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, string? expand = null);
        
        Task<(IEnumerable<T> Items, int TotalItems, int TotalPages)> GetPagedListAsync(
            string? search, 
            string? sort, 
            int page, 
            int pageSize, 
            string? expand,
            string? filter = null);
    }
}
