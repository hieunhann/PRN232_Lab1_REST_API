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
    [Route("api/subjects")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;
        private readonly IMapper _mapper;

        public SubjectsController(ISubjectService subjectService, IMapper mapper)
        {
            _subjectService = subjectService;
            _mapper = mapper;
        }

        /// <summary>
        /// Retrieves a specific subject record by ID.
        /// </summary>
        /// <param name="id">The unique identifier of the subject.</param>
        /// <param name="expand">Navigation properties to eagerly load.</param>
        /// <returns>An ApiResponse wrapping the SubjectResponse object.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubject(int id, [FromQuery] string? expand)
        {
            var businessModel = await _subjectService.GetSubjectByIdAsync(id, expand);
            if (businessModel == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Subject not found",
                    Errors = "404 Not Found"
                });
            }

            var responseModel = _mapper.Map<SubjectResponse>(businessModel);

            return Ok(new ApiResponse<SubjectResponse>
            {
                Success = true,
                Message = "Request processed successfully",
                Data = responseModel
            });
        }

        /// <summary>
        /// Retrieves a paginated list of subjects with sorting, searching, field selection, expansion, and filtering.
        /// </summary>
        /// <param name="request">The query request object containing search, sort, page, size, fields, expand, and filter parameters.</param>
        /// <returns>A paginated ApiResponse containing the shaped subject data.</returns>
        [HttpGet]
        public async Task<IActionResult> GetSubjects([FromQuery] ListQueryRequest request)
        {
            var result = await _subjectService.GetSubjectsAsync(request.search, request.sort, request.page, request.size, request.expand, request.filter);
            var responseModels = _mapper.Map<IEnumerable<SubjectResponse>>(result.Items);
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
        /// Creates a new subject record in the system.
        /// </summary>
        /// <param name="request">The subject creation payload containing the subject name.</param>
        /// <returns>The newly created subject wrapped in an ApiResponse with a 201 Created status.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectRequest request)
        {
            // Kiểm tra tính hợp lệ của dữ liệu đầu vào thông qua ModelState (Annotations Validation)
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Dữ liệu tạo môn học không hợp lệ!",
                    Errors = ModelState
                });
            }

            // Gọi service để lưu môn học mới vào database
            var createdBusiness = await _subjectService.AddSubjectAsync(request);
            var responseModel = _mapper.Map<SubjectResponse>(createdBusiness);

            // Trả về kết quả với mã trạng thái 201 Created và định vị URI của bản ghi vừa tạo
            return CreatedAtAction(
                nameof(GetSubject),
                new { id = responseModel.SubjectId },
                new ApiResponse<SubjectResponse>
                {
                    Success = true,
                    Message = "Tạo môn học thành công!",
                    Data = responseModel
                });
        }

        /// <summary>
        /// Fully updates an existing subject's details by their ID.
        /// </summary>
        /// <param name="id">The unique identifier of the subject to update.</param>
        /// <param name="request">The updated subject details payload.</param>
        /// <returns>An ApiResponse wrapping the updated subject data.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] SubjectRequest request)
        {
            // Kiểm tra tính hợp lệ của dữ liệu đầu vào thông qua ModelState
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Dữ liệu cập nhật môn học không hợp lệ!",
                    Errors = ModelState
                });
            }

            // Gọi service để cập nhật thông tin môn học
            var updatedBusiness = await _subjectService.UpdateSubjectAsync(id, request);
            if (updatedBusiness == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Môn học không tồn tại để cập nhật!",
                    Errors = "404 Not Found"
                });
            }

            var responseModel = _mapper.Map<SubjectResponse>(updatedBusiness);
            return Ok(new ApiResponse<SubjectResponse>
            {
                Success = true,
                Message = "Cập nhật môn học thành công!",
                Data = responseModel
            });
        }

        /// <summary>
        /// Permanently deletes a subject record from the system by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the subject to delete.</param>
        /// <returns>An ApiResponse indicating success or failure of the deletion.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            // Gọi service để thực hiện xóa và kiểm tra xem môn học có tồn tại/xóa được không
            var isDeleted = await _subjectService.DeleteSubjectAsync(id);
            if (!isDeleted)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Môn học không tồn tại hoặc không thể xóa!",
                    Errors = "404 Not Found"
                });
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Xóa môn học thành công!"
            });
        }
    }
}
