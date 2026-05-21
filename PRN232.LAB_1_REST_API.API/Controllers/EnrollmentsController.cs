using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LAB_1_REST_API.API.Extensions;
using PRN232.LAB_1_REST_API.Services.Models.Requests;
using PRN232.LAB_1_REST_API.Services.Models.Responses;
using PRN232.LAB_1_REST_API.Services.Interfaces;
using PRN232.LAB_1_REST_API.Services.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232.LAB_1_REST_API.API.Controllers
{
    [Route("api/enrollments")]
    [ApiController]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IMapper _mapper;

        public EnrollmentsController(IEnrollmentService enrollmentService, IMapper mapper)
        {
            _enrollmentService = enrollmentService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollment(int id, [FromQuery] string? expand)
        {
            expand ??= "Course.Enrollments.Student";

            var businessModel = await _enrollmentService.GetEnrollmentByIdAsync(id, expand);
            if (businessModel == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Enrollment not found",
                    Errors = "404 Not Found"
                });
            }

            var responseModel = _mapper.Map<EnrollmentResponse>(businessModel);

            return Ok(new ApiResponse<EnrollmentResponse>
            {
                Success = true,
                Message = "Request processed successfully",
                Data = responseModel
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetEnrollments([FromQuery] ListQueryRequest request)
        {
            var result = await _enrollmentService.GetEnrollmentsAsync(request.search, request.sort, request.page, request.size, request.expand, request.filter);
            var responseModels = _mapper.Map<IEnumerable<EnrollmentResponse>>(result.Items);
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
        /// POST: api/enrollments
        /// Tạo mới một bản ghi đăng ký môn học (Enrollment).
        /// </summary>
        /// <param name="request">Thông tin đăng ký môn học mới từ Body request</param>
        /// <returns>Bản ghi đăng ký vừa tạo kèm mã trạng thái 201 Created và Header Location</returns>
        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] EnrollmentRequest request)
        {
            // Kiểm tra tính hợp lệ của dữ liệu đầu vào thông qua ModelState (Annotations Validation)
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Dữ liệu tạo đăng ký học không hợp lệ!",
                    Errors = ModelState
                });
            }

            // Gọi service để thêm bản ghi đăng ký học vào CSDL
            var createdBusiness = await _enrollmentService.AddEnrollmentAsync(request);
            var responseModel = _mapper.Map<EnrollmentResponse>(createdBusiness);

            // Trả về kết quả với mã trạng thái 201 Created và định vị URI của bản ghi vừa tạo
            return CreatedAtAction(
                nameof(GetEnrollment),
                new { id = responseModel.EnrollmentId },
                new ApiResponse<EnrollmentResponse>
                {
                    Success = true,
                    Message = "Đăng ký môn học thành công!",
                    Data = responseModel
                });
        }

        /// <summary>
        /// PUT: api/enrollments/{id}
        /// Cập nhật thông tin bản ghi đăng ký môn học theo ID.
        /// </summary>
        /// <param name="id">Mã định danh của bản ghi đăng ký</param>
        /// <param name="request">Dữ liệu cập nhật mới</param>
        /// <returns>Bản ghi đăng ký môn học sau khi được cập nhật thành công</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnrollment(int id, [FromBody] EnrollmentRequest request)
        {
            // Kiểm tra tính hợp lệ của dữ liệu đầu vào thông qua ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Dữ liệu cập nhật đăng ký học không hợp lệ!",
                    Errors = ModelState
                });
            }

            // Gọi service cập nhật thông tin đăng ký học trong CSDL
            var updatedBusiness = await _enrollmentService.UpdateEnrollmentAsync(id, request);
            if (updatedBusiness == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Bản ghi đăng ký môn học không tồn tại để cập nhật!",
                    Errors = "404 Not Found"
                });
            }

            var responseModel = _mapper.Map<EnrollmentResponse>(updatedBusiness);
            return Ok(new ApiResponse<EnrollmentResponse>
            {
                Success = true,
                Message = "Cập nhật đăng ký học thành công!",
                Data = responseModel
            });
        }

        /// <summary>
        /// DELETE: api/enrollments/{id}
        /// Xóa bản ghi đăng ký môn học theo ID.
        /// </summary>
        /// <param name="id">Mã định danh của bản ghi đăng ký cần xóa</param>
        /// <returns>Thông điệp phản hồi kết quả xóa thành công hoặc lỗi</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            // Gọi service để xóa đăng ký học và kiểm tra kết quả xóa
            var isDeleted = await _enrollmentService.DeleteEnrollmentAsync(id);
            if (!isDeleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Bản ghi đăng ký học không tồn tại hoặc không thể xóa!",
                    Errors = "404 Not Found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa đăng ký môn học thành công!"
            });
        }

        /// <summary>
        /// GET: api/enrollments/{enrollmentId}/students
        /// Lấy tất cả sinh viên học cùng lớp (khóa học) của một enrollment theo EnrollmentId
        /// </summary>
        /// <param name="enrollmentId">Mã định danh đăng ký học</param>
        /// <returns>Danh sách sinh viên học cùng lớp</returns>
        [HttpGet("{enrollmentId}/students")]
        public async Task<IActionResult> GetStudentsByEnrollmentId(int enrollmentId)
        {
            // Lấy danh sách sinh viên cùng lớp từ service
            var students = await _enrollmentService.GetStudentsByEnrollmentIdAsync(enrollmentId);
            if (students == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Bản ghi đăng ký môn học không tồn tại!",
                    Errors = "404 Not Found"
                });
            }

            var responseModels = _mapper.Map<IEnumerable<StudentResponse>>(students);

            return Ok(new ApiResponse<IEnumerable<StudentResponse>>
            {
                Success = true,
                Message = "Lấy danh sách sinh viên cùng lớp thành công!",
                Data = responseModels
            });
        }

        /// <summary>
        /// GET: api/enrollments/{enrollmentId}/course
        /// Lấy khóa học theo enrollment và toàn bộ sinh viên đang tham gia khóa học đó
        /// </summary>
        [HttpGet("{enrollmentId}/course")]
        public async Task<IActionResult> GetCourseByEnrollmentId(int enrollmentId)
        {
            var course = await _enrollmentService.GetCourseByEnrollmentIdAsync(enrollmentId);
            if (course == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Bản ghi đăng ký môn học không tồn tại!",
                    Errors = "404 Not Found"
                });
            }

            var students = course.Enrollments
                .Where(enrollment => enrollment.Student != null)
                .GroupBy(enrollment => enrollment.Student!.StudentId)
                .Select(group => group.First().Student!)
                .Select(student => new StudentSummaryResponse
                {
                    StudentId = student.StudentId,
                    FullName = student.FullName,
                    Email = student.Email,
                    DateOfBirth = student.DateOfBirth
                })
                .ToList();

            var responseModel = new CourseWithStudentsResponse
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName,
                SemesterId = course.SemesterId,
                Semester = _mapper.Map<SemesterResponse?>(course.Semester),
                Students = students
            };

            return Ok(new ApiResponse<CourseWithStudentsResponse>
            {
                Success = true,
                Message = "Lấy khóa học theo enrollment thành công!",
                Data = responseModel
            });
        }
    }
}

