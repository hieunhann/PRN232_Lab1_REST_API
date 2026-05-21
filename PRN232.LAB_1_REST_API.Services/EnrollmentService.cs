using AutoMapper;
using PRN232.LAB_1_REST_API.Repositories;
using PRN232.LAB_1_REST_API.Repositories.Entities;
using PRN232.LAB_1_REST_API.Services.Interfaces;
using PRN232.LAB_1_REST_API.Services.Models;
using PRN232.LAB_1_REST_API.Services.Models.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.LAB_1_REST_API.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IGenericRepository<Enrollment> _repository;
        private readonly IMapper _mapper;

        public EnrollmentService(IGenericRepository<Enrollment> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EnrollmentBusinessModel?> GetEnrollmentByIdAsync(int id, string? expand)
        {
            var entity = await _repository.GetByIdAsync(id, expand);
            return _mapper.Map<EnrollmentBusinessModel>(entity);
        }

        public async Task<(IEnumerable<EnrollmentBusinessModel> Items, int TotalItems, int TotalPages)> GetEnrollmentsAsync(string? search, string? sort, int page, int pageSize, string? expand, string? filter = null)
        {
            var result = await _repository.GetPagedListAsync(search, sort, page, pageSize, expand, filter);
            var mappedItems = _mapper.Map<IEnumerable<EnrollmentBusinessModel>>(result.Items);
            return (mappedItems, result.TotalItems, result.TotalPages);
        }

        public async Task<EnrollmentBusinessModel> AddEnrollmentAsync(EnrollmentRequest request)
        {
            var entity = _mapper.Map<Enrollment>(request);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<EnrollmentBusinessModel>(entity);
        }

        public async Task<EnrollmentBusinessModel?> UpdateEnrollmentAsync(int id, EnrollmentRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            _mapper.Map(request, entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<EnrollmentBusinessModel>(entity);
        }

        public async Task<bool> DeleteEnrollmentAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            return await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<StudentBusinessModel>?> GetStudentsByEnrollmentIdAsync(int enrollmentId)
        {
            // Lấy thông tin Enrollment dựa trên ID để xác định CourseId tương ứng
            var enrollment = await _repository.GetByIdAsync(enrollmentId);
            if (enrollment == null) return null;

            int courseId = enrollment.CourseId;
            
            // Tìm tất cả các Enrollment cùng thuộc CourseId này và nạp thông tin Student
            var result = await _repository.GetPagedListAsync(
                search: null,
                sort: null,
                page: 1,
                pageSize: 9999, // Lấy toàn bộ danh sách đăng ký học của lớp
                expand: "Student",
                filter: $"CourseId == {courseId}"
            );

            // Trích xuất thông tin Student và ánh xạ sang Business Model
            var students = result.Items.Select(e => e.Student);
            return _mapper.Map<IEnumerable<StudentBusinessModel>>(students);
        }

        public async Task<CourseBusinessModel?> GetCourseByEnrollmentIdAsync(int enrollmentId)
        {
            var enrollment = await _repository.GetByIdAsync(enrollmentId, "Course.Enrollments.Student");
            return enrollment?.Course == null ? null : _mapper.Map<CourseBusinessModel>(enrollment.Course);
        }
    }
}
