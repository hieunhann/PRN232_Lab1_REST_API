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

        /// <summary>
        /// Thêm mới một thực thể vào cơ sở dữ liệu một cách bất đồng bộ
        /// </summary>
        /// <param name="entity">Thực thể cần thêm</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Lưu tất cả thay đổi đã thực hiện trong DbContext xuống cơ sở dữ liệu
        /// </summary>
        /// <returns>True nếu có ít nhất một bản ghi được thay đổi thành công, ngược lại False</returns>
        Task<bool> SaveChangesAsync();
    }
}
