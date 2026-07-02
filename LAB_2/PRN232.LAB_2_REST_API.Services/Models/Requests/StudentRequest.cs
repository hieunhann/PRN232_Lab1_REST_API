using System;
using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB_2_REST_API.Services.Models.Requests
{
    /// <summary>
    /// Model đại diện cho dữ liệu gửi lên khi tạo mới hoặc cập nhật thông tin học sinh.
    /// Validation được thực hiện bởi cả DataAnnotations và FluentValidation.
    /// </summary>
    public class StudentRequest
    {
        /// <summary>
        /// Họ và tên học sinh - Bắt buộc nhập.
        /// </summary>
        [Required(ErrorMessage = "FullName is required and cannot be empty")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "FullName must be between 2 and 100 characters")]
        public string FullName { get; set; } = null!;

        /// <summary>
        /// Địa chỉ Email - Bắt buộc nhập và phải đúng định dạng email.
        /// </summary>
        [Required(ErrorMessage = "Email is required and cannot be empty")]
        [EmailAddress(ErrorMessage = "Invalid Email format (e.g., abc@fpt.edu.vn)")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Ngày sinh của học sinh - Bắt buộc nhập.
        /// </summary>
        [Required(ErrorMessage = "DateOfBirth is required")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Mã số sinh viên theo chuẩn FPTU (ví dụ: SE19886, CE18793, IA20001).
        /// Format: 2 ký tự chữ in hoa + 5 chữ số. Không bắt buộc.
        /// </summary>
        [RegularExpression(@"^[A-Z]{2}\d{5}$",
            ErrorMessage = "StudentCode must match FPTU format: 2 uppercase letters + 5 digits (e.g., SE19886, CE18793)")]
        public string? StudentCode { get; set; }

        /// <summary>
        /// Số điện thoại của học sinh (không bắt buộc).
        /// </summary>
        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        public string? PhoneNumber { get; set; }
    }
}
