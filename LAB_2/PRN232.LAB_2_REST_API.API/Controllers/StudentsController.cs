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
    /// Students API v1 - Manage student information in the LMS system.
    /// Requires JWT Bearer Token authentication.
    /// </summary>
    [Route("api/v{version:apiVersion}/students")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize]
    [Produces("application/json", "application/xml")]
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
        /// [Admin Only] Retrieve all students (no pagination) - Admin only.
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllStudentsAdmin(
            [FromHeader(Name = "X-Request-Id")] string? requestId = null)
        {
            var result = await _studentService.GetStudentsAsync(null, null, 1, int.MaxValue, null, null);
            var responseModels = _mapper.Map<IEnumerable<StudentResponse>>(result.Items);

            return Ok(new ApiResponse<IEnumerable<StudentResponse>>
            {
                Success = true,
                Message = $"[Admin] Retrieved all {result.TotalItems} students successfully! RequestId: {requestId}",
                Data = responseModels
            });
        }

        /// <summary>
        /// Retrieve student details by ID.
        /// </summary>
        /// <param name="id">ID of the student to retrieve.</param>
        /// <param name="expand">Navigation properties to include (e.g., 'Enrollments.Course').</param>
        /// <param name="requestId">Request tracking ID from X-Request-Id header.</param>
        [HttpGet("{id:int}", Name = "GetStudentById")]
        public async Task<IActionResult> GetStudent(
            [FromRoute] int id,
            [FromQuery] string? expand,
            [FromHeader(Name = "X-Request-Id")] string? requestId = null)
        {
            expand ??= "Enrollments.Course";

            var businessModel = await _studentService.GetStudentByIdAsync(id, expand);
            if (businessModel == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Student not found with the provided ID.",
                    Errors = "404 Not Found"
                });
            }

            var responseModel = _mapper.Map<StudentResponse>(businessModel);

            return Ok(new ApiResponse<StudentResponse>
            {
                Success = true,
                Message = "Retrieved student information successfully!",
                Data = responseModel
            });
        }

        /// <summary>
        /// Retrieve paginated list of students with search, sort, and filter.
        /// </summary>
        /// <param name="request">Query parameters: search, sort, page, size, fields, expand, filter.</param>
        /// <param name="requestId">Request tracking ID from X-Request-Id header.</param>
        [HttpGet]
        public async Task<IActionResult> GetStudents(
            [FromQuery] ListQueryRequest request,
            [FromHeader(Name = "X-Request-Id")] string? requestId = null)
        {
            var result = await _studentService.GetStudentsAsync(request.Search, request.Sort, request.Page, request.Size, request.Expand, request.Filter);

            var responseModels = _mapper.Map<IEnumerable<StudentResponse>>(result.Items);

            // Dynamic Shaping (Selection): filter fields requested by client
            var shapedData = responseModels.ShapeData(request.Fields);

            var pagination = new PagedResponse
            {
                Page = request.Page,
                PageSize = request.Size,
                TotalItems = result.TotalItems,
                TotalPages = result.TotalPages
            };

            return Ok(new ApiResponse<object>
            {
                Pagination = pagination,
                Success = true,
                Message = "Retrieved list of students successfully!",
                Data = shapedData
            });
        }

        /// <summary>
        /// Create a new student in the system.
        /// </summary>
        /// <param name="request">Student information to create.</param>
        /// <param name="requestId">Request tracking ID from X-Request-Id header.</param>
        [HttpPost]
        public async Task<IActionResult> CreateStudent(
            [FromBody] StudentRequest request,
            [FromHeader(Name = "X-Request-Id")] string? requestId = null)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid data provided. Please check and try again.",
                    Errors = ModelState
                });
            }

            var createdBusiness = await _studentService.AddStudentAsync(request);
            var responseModel = _mapper.Map<StudentResponse>(createdBusiness);

            return CreatedAtAction(
                nameof(GetStudent),
                new { id = responseModel.StudentId },
                new ApiResponse<StudentResponse>
                {
                    Success = true,
                    Message = "Created student successfully!",
                    Data = responseModel
                });
        }

        /// <summary>
        /// Update a student's full information by ID.
        /// </summary>
        /// <param name="id">ID of the student to update.</param>
        /// <param name="request">Data to update.</param>
        /// <param name="requestId">Request tracking ID from X-Request-Id header.</param>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateStudent(
            [FromRoute] int id,
            [FromBody] StudentRequest request,
            [FromHeader(Name = "X-Request-Id")] string? requestId = null)
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

            var updatedBusiness = await _studentService.UpdateStudentAsync(id, request);
            if (updatedBusiness == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Student not found for update!",
                    Errors = "404 Not Found"
                });
            }

            var responseModel = _mapper.Map<StudentResponse>(updatedBusiness);
            return Ok(new ApiResponse<StudentResponse>
            {
                Success = true,
                Message = "Updated student information successfully!",
                Data = responseModel
            });
        }

        /// <summary>
        /// [Admin Only] Delete a student from the system by ID.
        /// </summary>
        /// <param name="id">ID of the student to delete.</param>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStudent([FromRoute] int id)
        {
            var isDeleted = await _studentService.DeleteStudentAsync(id);
            if (!isDeleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Student does not exist or cannot be deleted!",
                    Errors = "404 Not Found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Deleted student successfully!"
            });
        }

        /// <summary>
        /// Retrieve all courses a student is enrolled in.
        /// </summary>
        /// <param name="studentId">ID of the student.</param>
        [HttpGet("{studentId:int}/courses")]
        public async Task<IActionResult> GetCoursesByStudentId([FromRoute] int studentId)
        {
            var courses = await _studentService.GetCoursesByStudentIdAsync(studentId);
            if (courses == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Student does not exist!",
                    Errors = "404 Not Found"
                });
            }

            var responseModels = _mapper.Map<IEnumerable<CourseResponse>>(courses);

            return Ok(new ApiResponse<IEnumerable<CourseResponse>>
            {
                Success = true,
                Message = "Retrieved courses for student successfully!",
                Data = responseModels
            });
        }
    }
}
