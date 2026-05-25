using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LAB_1_REST_API.API.Extensions;
using PRN232.LAB_1_REST_API.Services.Models.Requests;
using PRN232.LAB_1_REST_API.Services.Models.Responses;
using PRN232.LAB_1_REST_API.Services.Interfaces;
using PRN232.LAB_1_REST_API.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB_1_REST_API.API.Controllers
{
    [Route("api/courses")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;

        public CoursesController(ICourseService courseService, IMapper mapper)
        {
            _courseService = courseService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a specific course's details by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the course.</param>
        /// <param name="expand">Navigation properties to eagerly load (e.g., 'Enrollments.Student').</param>
        /// <returns>An ApiResponse wrapping the CourseResponse object.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCourse(int id, [FromQuery] string? expand)
        {
            expand ??= "Enrollments.Student";

            var businessModel = await _courseService.GetCourseByIdAsync(id, expand);
            if (businessModel == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course not found",
                    Errors = "404 Not Found"
                });
            }

            var responseModel = _mapper.Map<CourseResponse>(businessModel);

            return Ok(new ApiResponse<CourseResponse>
            {
                Success = true,
                Message = "Request processed successfully",
                Data = responseModel
            });
        }

        /// <summary>
        /// Retrieves a paginated list of courses with sorting, searching, field selection, relation expansion, and filtering.
        /// </summary>
        /// <param name="request">The query request object containing search, sort, page, size, fields, expand, and filter parameters.</param>
        /// <returns>A paginated ApiResponse containing the shaped course data.</returns>
        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] ListQueryRequest request)
        {
            var result = await _courseService.GetCoursesAsync(request.search, request.sort, request.page, request.size, request.expand, request.filter);
            var responseModels = _mapper.Map<IEnumerable<CourseResponse>>(result.Items);
            var shapedData = responseModels.ShapeData(request.fields);

            return Ok(new ApiResponse<object>
            {
                Pagination = new PagedResponse { Page = request.page, PageSize = request.size, TotalItems = result.TotalItems, TotalPages = result.TotalPages },
                Success = true,
                Message = "Request processed successfully",
                Data = shapedData
            });
        }

        /// <summary>
        /// Creates a new course record in the system.
        /// </summary>
        /// <param name="request">The course creation payload containing course name and semester ID.</param>
        /// <returns>The newly created course wrapped in an ApiResponse with a 201 Created status.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Dữ liệu tạo mới không hợp lệ!",
                    Errors = ModelState
                });
            }

            var createdBusiness = await _courseService.AddCourseAsync(request);
            var responseModel = _mapper.Map<CourseResponse>(createdBusiness);

            return CreatedAtAction(
                nameof(GetCourse),
                new { id = responseModel.CourseId },
                new ApiResponse<CourseResponse>
                {
                    Success = true,
                    Message = "Tạo khóa học thành công!",
                    Data = responseModel
                });
        }

        /// <summary>
        /// Fully updates an existing course's details by their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the course to update.</param>
        /// <param name="request">The updated course payload.</param>
        /// <returns>An ApiResponse wrapping the updated course data.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Dữ liệu cập nhật không hợp lệ!",
                    Errors = ModelState
                });
            }

            var updatedBusiness = await _courseService.UpdateCourseAsync(id, request);
            if (updatedBusiness == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Khóa học không tồn tại để cập nhật!",
                    Errors = "404 Not Found"
                });
            }

            var responseModel = _mapper.Map<CourseResponse>(updatedBusiness);
            return Ok(new ApiResponse<CourseResponse>
            {
                Success = true,
                Message = "Cập nhật khóa học thành công!",
                Data = responseModel
            });
        }

        /// <summary>
        /// Permanently deletes a course record from the system by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the course to delete.</param>
        /// <returns>An ApiResponse indicating success or failure of the deletion.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var isDeleted = await _courseService.DeleteCourseAsync(id);
            if (!isDeleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Khóa học không tồn tại hoặc không thể xóa!",
                    Errors = "404 Not Found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa khóa học thành công!"
            });
        }

        /// <summary>
        /// Retrieves all students enrolled in a specific course by Course ID (returns a flat list with enrollment status and date).
        /// </summary>
        /// <param name="courseId">The unique identifier of the course.</param>
        /// <returns>An ApiResponse wrapping a collection of StudentInCourseResponse objects.</returns>
        [HttpGet("{courseId}/students")]
        public async Task<IActionResult> GetStudentsByCourseId(int courseId)
        {
            // Kiểm tra xem khóa học có tồn tại hay không
            var course = await _courseService.GetCourseByIdAsync(courseId, null);
            if (course == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Khóa học không tồn tại!",
                    Errors = "404 Not Found"
                });
            }

            // Lấy danh sách sinh viên của khóa học và ánh xạ sang Model Response phẳng
            var students = await _courseService.GetStudentsByCourseIdAsync(courseId);
            var responseModels = _mapper.Map<IEnumerable<StudentInCourseResponse>>(students);

            return Ok(new ApiResponse<IEnumerable<StudentInCourseResponse>>
            {
                Success = true,
                Message = "Lấy danh sách sinh viên của khóa học thành công!",
                Data = responseModels
            });
        }

        /// <summary>
        /// Retrieves all enrollments of a specific course, optionally expanding the student details.
        /// </summary>
        /// <param name="id">The unique identifier of the course.</param>
        /// <param name="expand">Navigation properties to eagerly load (e.g., 'Student').</param>
        /// <returns>An ApiResponse wrapping a collection of EnrollmentResponse objects.</returns>
        [HttpGet("{id}/enrollments")]
        public async Task<IActionResult> GetEnrollmentsByCourseId(int id, [FromQuery] string? expand)
        {
            string eagerLoadProperties = "Enrollments";
            if (!string.IsNullOrEmpty(expand) && expand.ToLower().Contains("student"))
            {
                eagerLoadProperties = "Enrollments.Student";
            }

            var course = await _courseService.GetCourseByIdAsync(id, eagerLoadProperties);
            if (course == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Khóa học không tồn tại!",
                    Errors = "404 Not Found"
                });
            }

            var responseModels = _mapper.Map<IEnumerable<EnrollmentResponse>>(course.Enrollments);

            return Ok(new ApiResponse<IEnumerable<EnrollmentResponse>>
            {
                Success = true,
                Message = "Lấy danh sách lượt đăng ký của khóa học thành công!",
                Data = responseModels
            });
        }
    }
}
