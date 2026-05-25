using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LAB_1_REST_API.API.Extensions;
using PRN232.LAB_1_REST_API.Services.Interfaces;
using PRN232.LAB_1_REST_API.Services.Models;
using PRN232.LAB_1_REST_API.Services.Models.Requests;
using PRN232.LAB_1_REST_API.Services.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB_1_REST_API.API.Controllers
{
    [Route("api/semesters")]
    [ApiController]
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _semesterService;
        private readonly IMapper _mapper;

        public SemestersController(ISemesterService semesterService, IMapper mapper)
        {
            _semesterService = semesterService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a specific semester record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the semester.</param>
        /// <param name="expand">Navigation properties to eagerly load (e.g., 'Courses').</param>
        /// <returns>An ApiResponse wrapping the SemesterResponse object.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSemester(int id, [FromQuery] string? expand)
        {
            var businessModel = await _semesterService.GetSemesterByIdAsync(id, expand);
            if (businessModel == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Semester not found",
                    Errors = "404 Not Found"
                });
            }

            var responseModel = _mapper.Map<SemesterResponse>(businessModel);

            return Ok(new ApiResponse<SemesterResponse>
            {
                Success = true,
                Message = "Request processed successfully",
                Data = responseModel
            });
        }

        /// <summary>
        /// Retrieves a paginated list of semesters with sorting, searching, field selection, expansion, and filtering.
        /// </summary>
        /// <param name="request">The query request object containing search, sort, page, size, fields, expand, and filter parameters.</param>
        /// <returns>A paginated ApiResponse containing the shaped semester data.</returns>
        [HttpGet]
        public async Task<IActionResult> GetSemesters([FromQuery] ListQueryRequest request)
        {
            var result = await _semesterService.GetSemestersAsync(request.search, request.sort, request.page, request.size, request.expand, request.filter);
            var responseModels = _mapper.Map<IEnumerable<SemesterResponse>>(result.Items);
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
        /// Creates a new semester record in the system.
        /// </summary>
        /// <param name="request">The semester creation payload containing semester name, start date, and end date.</param>
        /// <returns>The newly created semester wrapped in an ApiResponse with a 201 Created status.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateSemester([FromBody] SemesterRequest request)
        {
            // Kiểm tra tính hợp lệ của dữ liệu đầu vào thông qua ModelState (Annotations Validation)
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Dữ liệu tạo học kỳ không hợp lệ!",
                    Errors = ModelState
                });
            }

            // Gọi service để lưu học kỳ mới vào database
            var createdBusiness = await _semesterService.AddSemesterAsync(request);
            var responseModel = _mapper.Map<SemesterResponse>(createdBusiness);

            // Trả về kết quả với mã trạng thái 201 Created và định vị URI của bản ghi vừa tạo
            return CreatedAtAction(
                nameof(GetSemester),
                new { id = responseModel.SemesterId },
                new ApiResponse<SemesterResponse>
                {
                    Success = true,
                    Message = "Tạo học kỳ thành công!",
                    Data = responseModel
                });
        }

        /// <summary>
        /// Fully updates an existing semester's details by their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the semester to update.</param>
        /// <param name="request">The updated semester details payload.</param>
        /// <returns>An ApiResponse wrapping the updated semester data.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSemester(int id, [FromBody] SemesterRequest request)
        {
            // Kiểm tra tính hợp lệ của dữ liệu đầu vào thông qua ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Dữ liệu cập nhật học kỳ không hợp lệ!",
                    Errors = ModelState
                });
            }

            // Gọi service để cập nhật dữ liệu học kỳ
            var updatedBusiness = await _semesterService.UpdateSemesterAsync(id, request);
            if (updatedBusiness == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Học kỳ không tồn tại để cập nhật!",
                    Errors = "404 Not Found"
                });
            }

            var responseModel = _mapper.Map<SemesterResponse>(updatedBusiness);
            return Ok(new ApiResponse<SemesterResponse>
            {
                Success = true,
                Message = "Cập nhật học kỳ thành công!",
                Data = responseModel
            });
        }

        /// <summary>
        /// Permanently deletes a semester record from the system by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the semester to delete.</param>
        /// <returns>An ApiResponse indicating success or failure of the deletion.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSemester(int id)
        {
            // Gọi service để thực hiện xóa và kiểm tra xem bản ghi có tồn tại/xóa được không
            var isDeleted = await _semesterService.DeleteSemesterAsync(id);
            if (!isDeleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Học kỳ không tồn tại hoặc không thể xóa!",
                    Errors = "404 Not Found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa học kỳ thành công!"
            });
        }

        /// <summary>
        /// Retrieves all courses scheduled for a specific academic semester by Semester ID.
        /// </summary>
        /// <param name="semesterId">The unique identifier of the semester.</param>
        /// <returns>An ApiResponse wrapping a collection of CourseResponse objects.</returns>
        [HttpGet("{semesterId}/courses")]
        public async Task<IActionResult> GetCoursesBySemesterId(int semesterId)
        {
            // Gọi service lấy danh sách khóa học thuộc học kỳ
            var courses = await _semesterService.GetCoursesBySemesterIdAsync(semesterId);
            if (courses == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Học kỳ không tồn tại!",
                    Errors = "404 Not Found"
                });
            }

            var responseModels = _mapper.Map<IEnumerable<CourseResponse>>(courses);

            return Ok(new ApiResponse<IEnumerable<CourseResponse>>
            {
                Success = true,
                Message = "Lấy danh sách khóa học của học kỳ thành công!",
                Data = responseModels
            });
        }
    }
}
