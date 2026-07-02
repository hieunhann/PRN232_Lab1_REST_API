using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LAB_2_REST_API.API.Extensions;
using PRN232.LAB_2_REST_API.Services.Models.Requests;
using PRN232.LAB_2_REST_API.Services.Models.Responses;
using PRN232.LAB_2_REST_API.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB_2_REST_API.API.Controllers
{
    /// <summary>
    /// Courses API v1 - Quản lý khóa học trong hệ thống LMS.
    /// Requires JWT Bearer Token authentication.
    /// </summary>
    [Route("api/v{version:apiVersion}/courses")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Authorize]
    [Produces("application/json", "application/xml")]
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
        /// Retrieve course details by ID.
        /// </summary>
        /// <param name="id">ID của khóa học.</param>
        /// <param name="expand">Navigation properties cần load (ví dụ: 'Enrollments.Student').</param>
        /// <param name="requestId">Request tracking ID từ Header.</param>
        [HttpGet("{id:int}", Name = "GetCourseById")]
        public async Task<IActionResult> GetCourse(
            [FromRoute] int id,
            [FromQuery] string? expand,
            [FromHeader(Name = "X-Request-Id")] string? requestId = null)
        {
            expand ??= "Enrollments.Student";

            var businessModel = await _courseService.GetCourseByIdAsync(id, expand);
            if (businessModel == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course not found with the provided ID.",
                    Errors = "404 Not Found"
                });
            }

            var responseModel = _mapper.Map<CourseResponse>(businessModel);

            return Ok(new ApiResponse<CourseResponse>
            {
                Success = true,
                Message = "Retrieved course information successfully!",
                Data = responseModel
            });
        }

        /// <summary>
        /// Retrieve paginated list of courses with search, sort, and filter.
        /// </summary>
        /// <param name="request">Các tham số truy vấn.</param>
        /// <param name="requestId">Request tracking ID từ Header.</param>
        [HttpGet]
        public async Task<IActionResult> GetCourses(
            [FromQuery] ListQueryRequest request,
            [FromHeader(Name = "X-Request-Id")] string? requestId = null)
        {
            var result = await _courseService.GetCoursesAsync(request.Search, request.Sort, request.Page, request.Size, request.Expand, request.Filter);
            var responseModels = _mapper.Map<IEnumerable<CourseResponse>>(result.Items);
            var shapedData = responseModels.ShapeData(request.Fields);

            return Ok(new ApiResponse<object>
            {
                Pagination = new PagedResponse { Page = request.Page, PageSize = request.Size, TotalItems = result.TotalItems, TotalPages = result.TotalPages },
                Success = true,
                Message = "Retrieved list of courses successfully!",
                Data = shapedData
            });
        }

        /// <summary>
        /// Create a new course in the system.
        /// </summary>
        /// <param name="request">Course information to create.(tên và SemesterId).</param>
        /// <param name="requestId">Request tracking ID từ Header.</param>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCourse(
            [FromBody] CourseRequest request,
            [FromHeader(Name = "X-Request-Id")] string? requestId = null)
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
        /// Update a course's full information by ID.
        /// </summary>
        /// <param name="id">ID of the course to update.</param>
        /// <param name="request">Data to update.</param>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCourse(
            [FromRoute] int id,
            [FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid update data provided!",
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
        /// [Admin Only] Permanently delete a course from the system by ID.
        /// </summary>
        /// <param name="id">ID of the course to delete.</param>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse([FromRoute] int id)
        {
            var isDeleted = await _courseService.DeleteCourseAsync(id);
            if (!isDeleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Course does not exist or cannot be deleted!",
                    Errors = "404 Not Found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Deleted course successfully!"
            });
        }

        /// <summary>
        /// Retrieve list of students enrolled in a specific course (nested resource).
        /// </summary>
        /// <param name="courseId">ID của khóa học.</param>
        [HttpGet("{courseId:int}/students")]
        public async Task<IActionResult> GetStudentsByCourseId([FromRoute] int courseId)
        {
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
        /// Retrieve enrollments of a course, optionally expand student info.
        /// </summary>
        /// <param name="id">ID của khóa học.</param>
        /// <param name="expand">Navigation properties (ví dụ: 'Student').</param>
        [HttpGet("{id:int}/enrollments")]
        public async Task<IActionResult> GetEnrollmentsByCourseId(
            [FromRoute] int id,
            [FromQuery] string? expand)
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
