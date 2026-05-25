using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LAB_1_REST_API.API.Extensions;
using PRN232.LAB_1_REST_API.Services.Interfaces;
using PRN232.LAB_1_REST_API.Services.Models.Requests;
using PRN232.LAB_1_REST_API.Services.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB_1_REST_API.API.Controllers
{
    [Route("api/students")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;

        public StudentsController(IStudentService studentService, IMapper mapper)
        {
            _studentService = studentService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a specific student's details by ID, including related enrollments and courses.
        /// </summary>
        /// <param name="id">The unique identifier of the student.</param>
        /// <param name="expand">Navigation properties to eagerly load (e.g., 'Enrollments.Course').</param>
        /// <returns>An ApiResponse wrapping the StudentResponse object.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudent(int id, [FromQuery] string? expand)
        {
            expand ??= "Enrollments.Course";

            var businessModel = await _studentService.GetStudentByIdAsync(id, expand);
            if (businessModel == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Student not found",
                    Errors = "404 Not Found"
                });
            }

            var responseModel = _mapper.Map<StudentResponse>(businessModel);

            return Ok(new ApiResponse<StudentResponse>
            {
                Success = true,
                Message = "Request processed successfully",
                Data = responseModel
            });
        }

        /// <summary>
        /// Retrieves a paginated list of students with sorting, searching, field selection, expansion, and filtering capabilities.
        /// </summary>
        /// <param name="request">The query request object containing search, sort, page, size, fields, expand, and filter parameters.</param>
        /// <returns>A paginated ApiResponse containing the shaped student data.</returns>
        [HttpGet]
        public async Task<IActionResult> GetStudents([FromQuery] ListQueryRequest request)
        {
            var result = await _studentService.GetStudentsAsync(request.search, request.sort, request.page, request.size, request.expand, request.filter);
            
            var responseModels = _mapper.Map<IEnumerable<StudentResponse>>(result.Items);
            
            // Dynamic Shaping (Selection) để lọc các trường client yêu cầu
            var shapedData = responseModels.ShapeData(request.fields);

            var pagination = new PagedResponse
            {
                Page = request.page,
                PageSize = request.size,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            };

            return Ok(new ApiResponse<object>
            {
                Pagination = pagination,
                Success = true,
                Message = "Request processed successfully",
                Data = shapedData
            });
        }

        /// <summary>
        /// Creates a new student record in the system.
        /// </summary>
        /// <param name="request">The payload containing student details to create.</param>
        /// <returns>The newly created student wrapped in an ApiResponse with a 201 Created status.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentRequest request)
        {
            // Bước 1: Kiểm tra tính hợp lệ của Model dữ liệu (chạy bộ lọc DataAnnotations đã khai báo ở StudentRequest)
            if (!ModelState.IsValid)
            {
                // Nếu dữ liệu bị lỗi (ví dụ Email trống hoặc sai định dạng), lập tức trả về mã HTTP 400 Bad Request
                // Kết quả được đóng gói nhất quán trong cấu trúc ApiResponse với success = false và mô tả lỗi tại Errors
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Dữ liệu gửi lên không hợp lệ. Vui lòng kiểm tra lại!",
                    Errors = ModelState
                });
            }

            // Bước 2: Gọi tầng Business Service thực hiện logic thêm mới và lưu xuống DB
            var createdBusiness = await _studentService.AddStudentAsync(request);
            
            // Bước 3: Ánh xạ kết quả từ Business Model sang Response Model để trả về client
            var responseModel = _mapper.Map<StudentResponse>(createdBusiness);

            // Bước 4: Trả về mã HTTP 201 Created, cung cấp đường dẫn API GET chi tiết của đối tượng vừa tạo ở header "Location"
            // Đồng thời bọc kết quả trả về trong chuẩn ApiResponse thống nhất
            return CreatedAtAction(
                nameof(GetStudent), 
                new { id = responseModel.StudentId }, 
                new ApiResponse<StudentResponse>
                {
                    Success = true,
                    Message = "Tạo mới học sinh thành công!",
                    Data = responseModel
                });
        }

        /// <summary>
        /// Fully updates an existing student's details by their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the student to update.</param>
        /// <param name="request">The updated student details.</param>
        /// <returns>An ApiResponse wrapping the updated student data.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentRequest request)
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

            var updatedBusiness = await _studentService.UpdateStudentAsync(id, request);
            if (updatedBusiness == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Học sinh không tồn tại để cập nhật!",
                    Errors = "404 Not Found"
                });
            }

            var responseModel = _mapper.Map<StudentResponse>(updatedBusiness);
            return Ok(new ApiResponse<StudentResponse>
            {
                Success = true,
                Message = "Cập nhật thông tin học sinh thành công!",
                Data = responseModel
            });
        }

        /// <summary>
        /// Permanently deletes a student record from the system by their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the student to delete.</param>
        /// <returns>An ApiResponse indicating success or failure of the deletion.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var isDeleted = await _studentService.DeleteStudentAsync(id);
            if (!isDeleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Học sinh không tồn tại hoặc không thể xóa!",
                    Errors = "404 Not Found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa học sinh thành công!"
            });
        }

        /// <summary>
        /// Retrieves all courses that a specific student is registered/enrolled in.
        /// </summary>
        /// <param name="studentId">The unique identifier of the student.</param>
        /// <returns>An ApiResponse wrapping a collection of CourseResponse objects.</returns>
        [HttpGet("{studentId}/courses")]
        public async Task<IActionResult> GetCoursesByStudentId(int studentId)
        {
            // Lấy danh sách khóa học của học sinh từ service
            var courses = await _studentService.GetCoursesByStudentIdAsync(studentId);
            if (courses == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Học sinh không tồn tại!",
                    Errors = "404 Not Found"
                });
            }

            var responseModels = _mapper.Map<IEnumerable<CourseResponse>>(courses);

            return Ok(new ApiResponse<IEnumerable<CourseResponse>>
            {
                Success = true,
                Message = "Lấy danh sách khóa học của học sinh thành công!",
                Data = responseModels
            });
        }
    }
}
