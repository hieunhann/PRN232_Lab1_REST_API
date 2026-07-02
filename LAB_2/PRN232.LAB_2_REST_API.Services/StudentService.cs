using AutoMapper;
using PRN232.LAB_2_REST_API.Repositories;
using PRN232.LAB_2_REST_API.Repositories.Entities;
using PRN232.LAB_2_REST_API.Services.Interfaces;
using PRN232.LAB_2_REST_API.Services.Models;
using PRN232.LAB_2_REST_API.Services.Models.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.LAB_2_REST_API.Services
{
    public class StudentService : IStudentService
    {
        private readonly IGenericRepository<Student> _repository;
        private readonly IMapper _mapper;

        public StudentService(IGenericRepository<Student> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<StudentBusinessModel?> GetStudentByIdAsync(int id, string? expand)
        {
            var student = await _repository.GetByIdAsync(id, expand);
            return _mapper.Map<StudentBusinessModel>(student);
        }

        public async Task<(IEnumerable<StudentBusinessModel> Items, int TotalItems, int TotalPages)> GetStudentsAsync(string? search, string? sort, int page, int pageSize, string? expand, string? filter = null)
        {
            var result = await _repository.GetPagedListAsync(search, sort, page, pageSize, expand, filter);
            var mappedItems = _mapper.Map<IEnumerable<StudentBusinessModel>>(result.Items);
            return (mappedItems, result.TotalItems, result.TotalPages);
        }

        
        public async Task<StudentBusinessModel> AddStudentAsync(StudentRequest request)
        {
            // Bước 1: Dùng AutoMapper ánh xạ từ Request Model nhận được từ Controller sang Entity vật lý Student
            var studentEntity = _mapper.Map<Student>(request);
            
            // Bước 2: Gọi Repository để đưa đối tượng StudentEntity vào trạng thái Added trong DbContext
            await _repository.AddAsync(studentEntity);
            
            // Bước 3: Lưu các thay đổi xuống cơ sở dữ liệu (Database). 
            await _repository.SaveChangesAsync();
            
            // Bước 4: Ánh xạ thực thể Database đã có ID sang Business Model để trả về cho tầng Presentation
            return _mapper.Map<StudentBusinessModel>(studentEntity);
        }

        public async Task<StudentBusinessModel?> UpdateStudentAsync(int id, StudentRequest request)
        {
            var studentEntity = await _repository.GetByIdAsync(id);
            if (studentEntity == null) return null;

            _mapper.Map(request, studentEntity);
            _repository.Update(studentEntity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<StudentBusinessModel>(studentEntity);
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var studentEntity = await _repository.GetByIdAsync(id);
            if (studentEntity == null) return false;

            _repository.Delete(studentEntity);
            return await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<CourseBusinessModel>?> GetCoursesByStudentIdAsync(int studentId)
        {
            // Tải thông tin học sinh cùng danh sách đăng ký học và thông tin chi tiết khóa học tương ứng
            var student = await _repository.GetByIdAsync(studentId, "Enrollments.Course");
            if (student == null) return null;

            var courses = student.Enrollments.Select(e => e.Course);
            return _mapper.Map<IEnumerable<CourseBusinessModel>>(courses);
        }
    }
}
