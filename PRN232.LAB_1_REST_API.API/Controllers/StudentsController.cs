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

            var responseModel = _mapper.Map<StudentResponse>(businessModel);

            return Ok(new ApiResponse<StudentResponse>
            {
                Success = true,
                Message = "Request processed successfully",
                Data = responseModel
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents([FromQuery] ListQueryRequest request)
        {
            var result = await _studentService.GetStudentsAsync(request.Search, request.Sort, request.Page, request.Size, request.Expand, request.Filter);
            
            var responseModels = _mapper.Map<IEnumerable<StudentResponse>>(result.Items);
            
            // Dynamic Shaping (Selection) để lọc các trường client yêu cầu
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
                Message = "Request processed successfully",
                Data = shapedData
            });
        }

        /// <summary>
        /// API tạo mới học sinh
        /// </summary>
        /// <param name="request">Payload chứa thông tin học sinh do Client gửi lên</param>
        /// <returns>HTTP 201 Created kèm dữ liệu đã tạo nếu thành công, hoặc HTTP 400 Bad Request nếu dữ liệu lỗi</returns>
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
    }
}
