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
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IGenericRepository<Enrollment> _repository;
        private readonly IMapper _mapper;

        public EnrollmentService(IGenericRepository<Enrollment> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<EnrollmentBusinessModel?> GetEnrollmentByIdAsync(int id, string? expand)
        {
            var entity = await _repository.GetByIdAsync(id, expand);
            return _mapper.Map<EnrollmentBusinessModel>(entity);
        }

        public async Task<(IEnumerable<EnrollmentBusinessModel> Items, int TotalItems, int TotalPages)> GetEnrollmentsAsync(string? search, string? sort, int page, int pageSize, string? expand, string? filter = null)
        {
            var result = await _repository.GetPagedListAsync(search, sort, page, pageSize, expand, filter);
            var mappedItems = _mapper.Map<IEnumerable<EnrollmentBusinessModel>>(result.Items);
            return (mappedItems, result.TotalItems, result.TotalPages);
        }

        public async Task<EnrollmentBusinessModel> AddEnrollmentAsync(EnrollmentRequest request)
        {
            var entity = _mapper.Map<Enrollment>(request);
            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();
            return _mapper.Map<EnrollmentBusinessModel>(entity);
        }

        public async Task<EnrollmentBusinessModel?> UpdateEnrollmentAsync(int id, EnrollmentRequest request)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            _mapper.Map(request, entity);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<EnrollmentBusinessModel>(entity);
        }

        public async Task<bool> DeleteEnrollmentAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;

            _repository.Delete(entity);
            return await _repository.SaveChangesAsync();
        }
    }
}
