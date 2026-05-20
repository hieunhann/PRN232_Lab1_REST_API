using AutoMapper;
using PRN232.LAB_1_REST_API.Repositories;
using PRN232.LAB_1_REST_API.Repositories.Entities;
using PRN232.LAB_1_REST_API.Services.Interfaces;
using PRN232.LAB_1_REST_API.Services.Models;
using PRN232.LAB_1_REST_API.Services.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232.LAB_1_REST_API.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly IGenericRepository<Subject> _repository;
        private readonly IMapper _mapper;

        public SubjectService(IGenericRepository<Subject> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SubjectBusinessModel?> GetSubjectByIdAsync(int id, string? expand)
        {
            var entity = await _repository.GetByIdAsync(id, expand);
            return _mapper.Map<SubjectBusinessModel>(entity);
        }

        public async Task<(IEnumerable<SubjectBusinessModel> Items, int TotalItems, int TotalPages)> GetSubjectsAsync(string? search, string? sort, int page, int pageSize, string? expand, string? filter = null)
        {
            var result = await _repository.GetPagedListAsync(search, sort, page, pageSize, expand, filter);
            var mappedItems = _mapper.Map<IEnumerable<SubjectBusinessModel>>(result.Items);
            return (mappedItems, result.TotalItems, result.TotalPages);
        }

        public async Task<SubjectBusinessModel> AddSubjectAsync(SubjectRequest request)
        {
            var entity = _mapper.Map<Subject>(request);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<SubjectBusinessModel>(entity);
        }

        public async Task<SubjectBusinessModel?> UpdateSubjectAsync(int id, SubjectRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            _mapper.Map(request, entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<SubjectBusinessModel>(entity);
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            return await _repository.SaveChangesAsync();
        }
    }
}
