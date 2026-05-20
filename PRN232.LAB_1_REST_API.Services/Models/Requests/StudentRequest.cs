using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB_1_REST_API.Services.Models.Requests
{
    /// <summary>
    /// Model đại diện cho dữ liệu gửi lên khi tạo mới hoặc cập nhật thông tin học sinh
    /// </summary>
    public class StudentRequest
    {
        /// <summary>
        /// Họ và tên học sinh - Bắt buộc nhập
        /// </summary>
        [Required(ErrorMessage = "FullName is required và không được để trống")]
        [StringLength(100, ErrorMessage = "Tên học sinh không được dài quá 100 ký tự")]
        public string FullName { get; set; } = null!;

        /// <summary>
        /// Địa chỉ Email - Bắt buộc nhập và phải đúng định dạng email
        /// </summary>
        [Required(ErrorMessage = "Email is required và không được để trống")]
        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ (ví dụ mẫu: abc@fpt.edu.vn)")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Ngày sinh của học sinh - Bắt buộc nhập
        /// </summary>
        [Required(ErrorMessage = "DateOfBirth is required")]
        public DateTime DateOfBirth { get; set; }
    }
}
