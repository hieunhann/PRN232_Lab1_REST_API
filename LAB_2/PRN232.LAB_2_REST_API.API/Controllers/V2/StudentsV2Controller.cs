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

namespace PRN232.LAB_2_REST_API.API.Controllers.V2
{
    /// <summary>
    /// Students API v2 - Advanced version with Age, StudentCode, and PhoneNumber.
    /// Requires JWT Bearer Token authentication.
    /// </summary>
    [Route("api/v{version:apiVersion}/students")]
    [ApiController]
    [ApiVersion("2.0")]
    [Authorize]
    [Produces("application/json", "application/xml")]
    public class StudentsV2Controller : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;

        public StudentsV2Controller(IStudentService studentService, IMapper mapper)
        {
            _studentService = studentService;
            _mapper = mapper;
        }

        /// <summary>
        /// [v2] Retrieve student details including auto-calculated Age.
        /// </summary>
        /// <param name="id">ID of the student.</param>
        /// <param name="expand">Navigation properties to include.</param>
        /// <param name="requestId">Request tracking ID.</param>
        [HttpGet("{id:int}", Name = "GetStudentByIdV2")]
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

            var responseModel = _mapper.Map<StudentV2Response>(businessModel);

            return Ok(new ApiResponse<StudentV2Response>
            {
                Success = true,
                Message = "[v2] Retrieved student information successfully!",
                Data = responseModel
            });
        }

        /// <summary>
        /// [v2] Retrieve paginated list of students, including Age and StudentCode.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetStudents(
            [FromQuery] ListQueryRequest request,
            [FromHeader(Name = "X-Request-Id")] string? requestId = null)
        {
            var result = await _studentService.GetStudentsAsync(request.Search, request.Sort, request.Page, request.Size, request.Expand, request.Filter);

            var responseModels = _mapper.Map<IEnumerable<StudentV2Response>>(result.Items);
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
                Message = "[v2] Retrieved list of students successfully!",
                Data = shapedData
            });
        }

        /// <summary>
        /// [v2] Create a new student with StudentCode and PhoneNumber.
        /// </summary>
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
                    Message = "Invalid data provided.",
                    Errors = ModelState
                });
            }

            var createdBusiness = await _studentService.AddStudentAsync(request);
            var responseModel = _mapper.Map<StudentV2Response>(createdBusiness);

            return CreatedAtAction(
                nameof(GetStudent),
                new { id = responseModel.StudentId },
                new ApiResponse<StudentV2Response>
                {
                    Success = true,
                    Message = "[v2] Created student successfully!",
                    Data = responseModel
                });
        }

        /// <summary>
        /// [v2][Admin Only] Delete a student by ID.
        /// </summary>
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
                Message = "[v2] Deleted student successfully!"
            });
        }
    }
}
