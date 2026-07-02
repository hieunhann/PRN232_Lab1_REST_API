using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LAB_2_REST_API.API.Extensions;
using PRN232.LAB_2_REST_API.Services.Interfaces;
using PRN232.LAB_2_REST_API.Services.Models.Requests;
using PRN232.LAB_2_REST_API.Services.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB_2_REST_API.API.Controllers
{
    /// <summary>
    /// Semesters API v1/v2 - Quản lý học kỳ trong hệ thống LMS.
    /// Requires JWT Bearer Token authentication.
    /// </summary>
    [Route("api/v{version:apiVersion}/semesters")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Authorize]
    [Produces("application/json", "application/xml")]
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
        [HttpGet("{id:int}", Name = "GetSemesterById")]
        public async Task<IActionResult> GetSemester(
            [FromRoute] int id,
            [FromQuery] string? expand,
            [FromHeader(Name = "X-Request-Id")] string? requestId = null)
        {
            var businessModel = await _semesterService.GetSemesterByIdAsync(id, expand);
            if (businessModel == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Semester not found with the provided ID.",
                    Errors = "404 Not Found"
                });
            }

            var responseModel = _mapper.Map<SemesterResponse>(businessModel);

            return Ok(new ApiResponse<SemesterResponse>
            {
                Success = true,
                Message = "Retrieved semester information successfully!",
                Data = responseModel
            });
        }

        /// <summary>
        /// Retrieves a paginated list of semesters with sorting, searching, field selection, expansion, and filtering.
        /// </summary>
        /// <param name="request">The query request object containing search, sort, page, size, fields, expand, and filter parameters.</param>
        /// <returns>A paginated ApiResponse containing the shaped semester data.</returns>
        [HttpGet]
        public async Task<IActionResult> GetSemesters(
            [FromQuery] ListQueryRequest request,
            [FromHeader(Name = "X-Request-Id")] string? requestId = null)
        {
            var result = await _semesterService.GetSemestersAsync(request.Search, request.Sort, request.Page, request.Size, request.Expand, request.Filter);
            var responseModels = _mapper.Map<IEnumerable<SemesterResponse>>(result.Items);
            var shapedData = responseModels.ShapeData(request.Fields);

            return Ok(new ApiResponse<object>
            {
                Pagination = new PagedResponse { Page = request.Page, PageSize = request.Size, TotalItems = result.TotalItems, TotalPages = result.TotalPages },
                Success = true,
                Message = "Retrieved list of semesters successfully!",
                Data = shapedData
            });
        }

        /// <summary>
        /// Creates a new semester record in the system.
        /// </summary>
        /// <param name="request">The semester creation payload containing semester name, start date, and end date.</param>
        /// <returns>The newly created semester wrapped in an ApiResponse with a 201 Created status.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
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
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSemester([FromRoute] int id, [FromBody] SemesterRequest request)
        {
            // Kiểm tra tính hợp lệ của dữ liệu đầu vào thông qua ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Data to update.học kỳ không hợp lệ!",
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
                Message = "Updated semester successfully!",
                Data = responseModel
            });
        }

        /// <summary>
        /// Permanently deletes a semester record from the system by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the semester to delete.</param>
        /// <returns>An ApiResponse indicating success or failure of the deletion.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSemester([FromRoute] int id)
        {
            // Gọi service để thực hiện xóa và kiểm tra xem bản ghi có tồn tại/xóa được không
            var isDeleted = await _semesterService.DeleteSemesterAsync(id);
            if (!isDeleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Semester does not exist or cannot be deleted!",
                    Errors = "404 Not Found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Deleted semester successfully!"
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
