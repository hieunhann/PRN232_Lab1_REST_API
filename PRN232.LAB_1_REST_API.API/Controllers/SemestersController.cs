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

        [HttpGet]
        public async Task<IActionResult> GetSemesters([FromQuery] ListQueryRequest request)
        {
            var result = await _semesterService.GetSemestersAsync(request.Search, request.Sort, request.Page, request.Size, request.Expand, request.Filter);
            var responseModels = _mapper.Map<IEnumerable<SemesterResponse>>(result.Items);
            var shapedData = responseModels.ShapeData(request.Fields);

            return Ok(new ApiResponse<object>
            {
                Pagination = new PagedResponse { Page = request.Page, PageSize = request.Size, TotalItems = result.TotalItems, TotalPages = result.TotalPages },
                Success = true,
                Message = "Request processed successfully",
                Data = shapedData
            });
        }
    }
}
