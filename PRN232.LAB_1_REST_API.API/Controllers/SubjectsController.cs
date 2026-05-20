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

        [HttpGet]
        public async Task<IActionResult> GetSubjects([FromQuery] ListQueryRequest request)
        {
            var result = await _subjectService.GetSubjectsAsync(request.Search, request.Sort, request.Page, request.Size, request.Expand, request.Filter);
            var responseModels = _mapper.Map<IEnumerable<SubjectResponse>>(result.Items);
            var shapedData = responseModels.ShapeData(request.Fields);

            return Ok(new ApiResponse<object>
            {
                Pagination = new PagedResponse { Page = request.Page, PageSize = request.Size, TotalItems = result.TotalItems, TotalPages = result.TotalPages },
                Success = true,
                Message = "Request processed successfully",
                Data = shapedData
            });
        }

        /// <summary>
        /// POST: api/subjects
        /// Tạo mới một môn học (Subject).
        /// </summary>
        /// <param name="request">Thông tin môn học mới từ Body request</param>
        /// <returns>Thông tin môn học vừa tạo kèm mã trạng thái 201 Created và Header Location</returns>
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
        /// PUT: api/subjects/{id}
        /// Cập nhật thông tin môn học theo ID.
        /// </summary>
        /// <param name="id">Mã định danh môn học cần cập nhật</param>
        /// <param name="request">Dữ liệu môn học mới</param>
        /// <returns>Thông tin môn học sau khi cập nhật thành công</returns>
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
        /// DELETE: api/subjects/{id}
        /// Xóa môn học theo ID.
        /// </summary>
        /// <param name="id">Mã định danh môn học cần xóa</param>
        /// <returns>Thông điệp phản hồi kết quả xóa thành công hoặc lỗi</returns>
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
