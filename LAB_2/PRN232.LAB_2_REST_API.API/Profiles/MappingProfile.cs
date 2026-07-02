using AutoMapper;
using PRN232.LAB_2_REST_API.Repositories.Entities;
using PRN232.LAB_2_REST_API.Services.Models;
using PRN232.LAB_2_REST_API.Services.Models.Requests;
using PRN232.LAB_2_REST_API.Services.Models.Responses;

namespace PRN232.LAB_2_REST_API.API.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Request Model -> Database Entity (for POST create & PUT update)
            CreateMap<StudentRequest, Student>();
            CreateMap<CourseRequest, Course>();
            CreateMap<EnrollmentRequest, Enrollment>();
            CreateMap<SemesterRequest, Semester>();
            CreateMap<SubjectRequest, Subject>();

            // Entity to Business Model
            CreateMap<Student, StudentBusinessModel>();
            CreateMap<Enrollment, EnrollmentBusinessModel>();
            CreateMap<Course, CourseBusinessModel>();
            CreateMap<Subject, SubjectBusinessModel>();
            CreateMap<Semester, SemesterBusinessModel>();
            
            // Business Model to API Response Model
            CreateMap<StudentBusinessModel, StudentResponse>();
            CreateMap<StudentBusinessModel, StudentV2Response>(); // v2: thêm Age, StudentCode, PhoneNumber
            CreateMap<EnrollmentBusinessModel, EnrollmentResponse>();
            CreateMap<CourseBusinessModel, CourseResponse>();
            CreateMap<SubjectBusinessModel, SubjectResponse>();
            CreateMap<SemesterBusinessModel, SemesterResponse>();
            CreateMap<StudentInCourseBusinessModel, StudentInCourseResponse>();
        }
    }
}

