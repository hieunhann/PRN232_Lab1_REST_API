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
    public class CourseService : ICourseService
    {
        private readonly IGenericRepository<Course> _repository;
        private readonly IMapper _mapper;

        public CourseService(IGenericRepository<Course> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CourseBusinessModel?> GetCourseByIdAsync(int id, string? expand)
        {
            var entity = await _repository.GetByIdAsync(id, expand);
            return _mapper.Map<CourseBusinessModel>(entity);
        }

        public async Task<(IEnumerable<CourseBusinessModel> Items, int TotalItems, int TotalPages)> GetCoursesAsync(string? search, string? sort, int page, int pageSize, string? expand, string? filter = null)
        {
            var result = await _repository.GetPagedListAsync(search, sort, page, pageSize, expand, filter);
            var mappedItems = _mapper.Map<IEnumerable<CourseBusinessModel>>(result.Items);
            return (mappedItems, result.TotalItems, result.TotalPages);
        }

        public async Task<CourseBusinessModel> AddCourseAsync(CourseRequest request)
        {
            var entity = _mapper.Map<Course>(request);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<CourseBusinessModel>(entity);
        }

        public async Task<CourseBusinessModel?> UpdateCourseAsync(int id, CourseRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            _mapper.Map(request, entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<CourseBusinessModel>(entity);
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            return await _repository.SaveChangesAsync();
        }

        public async Task<IEnumerable<StudentInCourseBusinessModel>> GetStudentsByCourseIdAsync(int courseId)
        {
            // Tải khóa học kèm theo danh sách Enrollments và thông tin chi tiết từng Student
            var course = await _repository.GetByIdAsync(courseId, "Enrollments.Student");
            if (course == null) return Enumerable.Empty<StudentInCourseBusinessModel>();

            // Ánh xạ phẳng sang StudentInCourseBusinessModel
            var studentsInCourse = new List<StudentInCourseBusinessModel>();
            foreach (var e in course.Enrollments)
            {
                if (e.Student != null)
                {
                    studentsInCourse.Add(new StudentInCourseBusinessModel
                    {
                        StudentId = e.Student.StudentId,
                        FullName = e.Student.FullName,
                        Email = e.Student.Email,
                        DateOfBirth = e.Student.DateOfBirth,
                        EnrollmentStatus = e.Status,
                        EnrollDate = e.EnrollDate
                    });
                }
            }
            return studentsInCourse;
        }
    }
}
