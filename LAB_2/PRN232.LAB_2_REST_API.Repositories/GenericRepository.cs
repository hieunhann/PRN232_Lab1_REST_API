using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace PRN232.LAB_2_REST_API.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly LmsDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(LmsDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(int id, string? expand = null)
        {
            IQueryable<T> query = _dbSet;

            if (!string.IsNullOrWhiteSpace(expand))
            {
                var properties = expand.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var prop in properties)
                {
                    string formattedProp = NormalizeIncludePath(prop);
                    query = query.Include(formattedProp);
                }
            }

            var keyName = _context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()?.Properties
                .Select(x => x.Name).FirstOrDefault();

            if (keyName != null)
            {
                return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, keyName) == id);
            }

            return null;
        }

        public async Task<(IEnumerable<T> Items, int TotalItems, int TotalPages)> GetPagedListAsync(
            string? search, 
            string? sort, 
            int page, 
            int pageSize, 
            string? expand,
            string? filter = null)
        {
            IQueryable<T> query = _dbSet;

            // 0. Filtering
            if (!string.IsNullOrWhiteSpace(filter))
            {
                query = query.Where(filter);
            }

            // 1. Expansion
            if (!string.IsNullOrWhiteSpace(expand))
            {
                var properties = expand.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var prop in properties)
                {
                    string formattedProp = NormalizeIncludePath(prop);
                    query = query.Include(formattedProp);
                }
            }

            // 2. Searching
            if (!string.IsNullOrWhiteSpace(search))
            {
                var stringProperties = typeof(T).GetProperties()
                    .Where(p => p.PropertyType == typeof(string))
                    .Select(p => p.Name).ToList();

                if (stringProperties.Any())
                {
                    var searchPredicate = string.Join(" || ", stringProperties.Select(p => $"{p}.Contains(@0)"));
                    query = query.Where(searchPredicate, search);
                }
            }

            // 3. Sorting
            if (!string.IsNullOrWhiteSpace(sort))
            {
                var sortFields = sort.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var sortExpressions = new List<string>();
                foreach (var field in sortFields)
                {
                    var trimmed = field.Trim();
                    string fieldName = trimmed.StartsWith("-") ? trimmed.Substring(1) : trimmed;
                    string formattedField = char.ToUpper(fieldName[0]) + fieldName.Substring(1);

                    if (trimmed.StartsWith("-"))
                    {
                        sortExpressions.Add($"{formattedField} descending");
                    }
                    else
                    {
                        sortExpressions.Add(formattedField);
                    }
                }
                query = query.OrderBy(string.Join(", ", sortExpressions));
            }

            // 4. Paging
            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalItems, totalPages);
        }

        /// <summary>
        /// Thêm thực thể vào DbContext một cách bất đồng bộ
        /// </summary>
        public async Task AddAsync(T entity)
        {
            // Sử dụng DbSet.AddAsync để đưa thực thể vào trạng thái Added
            await _dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Cập nhật thực thể trong DbContext
        /// </summary>
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        /// <summary>
        /// Xóa thực thể khỏi DbContext
        /// </summary>
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        /// <summary>
        /// Thực hiện lưu toàn bộ thay đổi trong DbContext xuống DB vật lý
        /// </summary>
        public async Task<bool> SaveChangesAsync()
        {
            // Trả về true nếu số lượng dòng bị ảnh hưởng lớn hơn 0
            return await _context.SaveChangesAsync() > 0;
        }

        private static string NormalizeIncludePath(string path)
        {
            var segments = path
                .Split('.', StringSplitOptions.RemoveEmptyEntries)
                .Select(segment => segment.Trim())
                .Where(segment => !string.IsNullOrWhiteSpace(segment))
                .Select(segment => char.ToUpper(segment[0]) + segment.Substring(1));

            return string.Join('.', segments);
        }
    }
}
