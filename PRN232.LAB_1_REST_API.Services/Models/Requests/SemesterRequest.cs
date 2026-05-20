using System;

namespace PRN232.LAB_1_REST_API.Services.Models.Requests
{
    public class SemesterRequest
    {
        public string SemesterName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
