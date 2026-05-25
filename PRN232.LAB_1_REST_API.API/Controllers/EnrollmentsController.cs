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

        /// <summary>
        /// Retrieves a specific enrollment record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the enrollment.</param>
        /// <param name="expand">Navigation properties to eagerly load (e.g., 'Course.Enrollments.Student').</param>
        /// <returns>An ApiResponse wrapping the EnrollmentResponse object.</returns>
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

        /// <summary>
        /// Retrieves a paginated list of enrollments with sorting, searching, field selection, expansion, and filtering.
        /// </summary>
        /// <param name="request">The query request object containing search, sort, page, size, fields, expand, and filter parameters.</param>
        /// <returns>A paginated ApiResponse containing the shaped enrollment data.</returns>
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
        /// Creates a new enrollment record (registers a student to a course).
        /// </summary>
        /// <param name="request">The enrollment request payload containing student ID, course ID, and enrollment date.</param>
        /// <returns>The newly created enrollment wrapped in an ApiResponse with a 201 Created status.</returns>
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
        /// Fully updates an existing enrollment record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the enrollment record to update.</param>
        /// <param name="request">The updated enrollment payload.</param>
        /// <returns>An ApiResponse wrapping the updated enrollment data.</returns>
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
        /// Permanently deletes an enrollment record from the system by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the enrollment to delete.</param>
        /// <returns>An ApiResponse indicating success or failure of the deletion.</returns>
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
        /// Retrieves all classmates enrolled in the same course as the specified enrollment.
        /// </summary>
        /// <param name="enrollmentId">The unique identifier of the enrollment record.</param>
        /// <returns>An ApiResponse wrapping a collection of classmate StudentResponse objects.</returns>
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
        /// Retrieves the course details and all its enrolled students associated with the specified enrollment ID.
        /// </summary>
        /// <param name="enrollmentId">The unique identifier of the enrollment record.</param>
        /// <returns>An ApiResponse wrapping a CourseWithStudentsResponse object.</returns>
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

