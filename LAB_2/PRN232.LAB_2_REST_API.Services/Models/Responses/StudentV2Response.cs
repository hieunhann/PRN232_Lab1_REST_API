using System;
using System.Collections.Generic;

namespace PRN232.LAB_2_REST_API.Services.Models.Responses
{
    /// <summary>
    /// Response model v2 - Extended with Age, StudentCode, and PhoneNumber fields.
    /// </summary>
    public class StudentV2Response
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Age auto-calculated from DateOfBirth (v2 only).
        /// </summary>
        public int Age => DateTime.Today.Year - DateOfBirth.Year -
                         (DateTime.Today < DateOfBirth.AddYears(DateTime.Today.Year - DateOfBirth.Year) ? 1 : 0);

        /// <summary>
        /// FPTU student code (v2 only).
        /// </summary>
        public string? StudentCode { get; set; }

        /// <summary>
        /// Phone number (v2 only).
        /// </summary>
        public string? PhoneNumber { get; set; }

        public ICollection<EnrollmentResponse> Enrollments { get; set; } = new List<EnrollmentResponse>();
    }
}
