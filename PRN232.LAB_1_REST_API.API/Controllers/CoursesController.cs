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
        /// GET: api/courses/{courseId}/students
        /// Lấy danh sách tất cả các sinh viên đã đăng ký học khóa học theo CourseId
        /// </summary>
        /// <param name="courseId">Mã định danh khóa học</param>
        /// <returns>Danh sách sinh viên</returns>
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

            // Lấy danh sách sinh viên của khóa học và ánh xạ sang Model Response
            var students = await _courseService.GetStudentsByCourseIdAsync(courseId);
            var responseModels = _mapper.Map<IEnumerable<StudentResponse>>(students);

            return Ok(new ApiResponse<IEnumerable<StudentResponse>>
            {
                Success = true,
                Message = "Lấy danh sách sinh viên của khóa học thành công!",
                Data = responseModels
            });
        }
    }
}
