using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LAB_1_REST_API.API.Extensions;
using PRN232.LAB_1_REST_API.API.Models;
using PRN232.LAB_1_REST_API.Services.Interfaces;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudent(int id, [FromQuery] string? expand)
        {
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

            var responseModel = _mapper.Map<StudentResponseModel>(businessModel);

            return Ok(new ApiResponse<StudentResponseModel>
            {
                Success = true,
                Message = "Request processed successfully",
                Data = responseModel
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents([FromQuery] PaginationRequestModel request)
        {
            var result = await _studentService.GetStudentsAsync(request.Search, request.Sort, request.Page, request.Size, request.Expand, request.Filter);
            
            var responseModels = _mapper.Map<IEnumerable<StudentResponseModel>>(result.Items);
            
            // Dynamic Shaping (Selection) để lọc các trường client yêu cầu
            var shapedData = responseModels.ShapeData(request.Fields);

            var pagination = new PaginationMetadata
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
                Message = "Request processed successfully",
                Data = shapedData
            });
        }
    }
}
