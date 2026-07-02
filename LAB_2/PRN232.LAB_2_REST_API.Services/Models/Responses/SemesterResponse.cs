using System;
using System.Collections.Generic;

namespace PRN232.LAB_2_REST_API.Services.Models.Responses
{
    public class SemesterResponse
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        
        public ICollection<CourseResponse> Courses { get; set; } = new List<CourseResponse>();
    }
}
