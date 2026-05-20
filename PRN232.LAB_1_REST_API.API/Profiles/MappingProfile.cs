using AutoMapper;
using PRN232.LAB_1_REST_API.Repositories.Entities;
using PRN232.LAB_1_REST_API.Services.Models;
using PRN232.LAB_1_REST_API.Services.Models.Responses;

namespace PRN232.LAB_1_REST_API.API.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entity to Business Model
            CreateMap<Student, StudentBusinessModel>();
            CreateMap<Enrollment, EnrollmentBusinessModel>();
            CreateMap<Course, CourseBusinessModel>();
            CreateMap<Subject, SubjectBusinessModel>();
            CreateMap<Semester, SemesterBusinessModel>();
            
            // Business Model to API Response Model
            CreateMap<StudentBusinessModel, StudentResponse>();
            CreateMap<EnrollmentBusinessModel, EnrollmentResponse>();
            CreateMap<CourseBusinessModel, CourseResponse>();
            CreateMap<SubjectBusinessModel, SubjectResponse>();
            CreateMap<SemesterBusinessModel, SemesterResponse>();
        }
    }
}

