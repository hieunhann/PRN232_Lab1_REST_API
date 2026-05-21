using System;

namespace PRN232.LAB_1_REST_API.Services.Models.Responses
{
    public class StudentSummaryResponse
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
    }
}