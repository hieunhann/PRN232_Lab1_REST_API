using AutoMapper;
using PRN232.LAB_1_REST_API.Repositories;
using PRN232.LAB_1_REST_API.Repositories.Entities;
using PRN232.LAB_1_REST_API.Services.Interfaces;
using PRN232.LAB_1_REST_API.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB_1_REST_API.Services
{
    public class StudentService : IStudentService
    {
        private readonly IGenericRepository<Student> _repository;
        private readonly IMapper _mapper;

        public StudentService(IGenericRepository<Student> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<StudentBusinessModel?> GetStudentByIdAsync(int id, string? expand)
        {
            var student = await _repository.GetByIdAsync(id, expand);
            return _mapper.Map<StudentBusinessModel>(student);
        }

        public async Task<(IEnumerable<StudentBusinessModel> Items, int TotalItems, int TotalPages)> GetStudentsAsync(string? search, string? sort, int page, int pageSize, string? expand, string? filter = null)
        {
            var result = await _repository.GetPagedListAsync(search, sort, page, pageSize, expand, filter);
            var mappedItems = _mapper.Map<IEnumerable<StudentBusinessModel>>(result.Items);
            return (mappedItems, result.TotalItems, result.TotalPages);
        }
    }
}
