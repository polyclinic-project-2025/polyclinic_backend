using PolyclinicApplication.Common.Results;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;

namespace PolyclinicApplication.Services.Implementations
{
    public class DepartmentHeadService : IDepartmentHeadService
    {
        private readonly IDepartmentHeadRepository _departmentHeadRepository;

        public DepartmentHeadService(IDepartmentHeadRepository departmentHeadRepository)
        {
            _departmentHeadRepository = departmentHeadRepository;
        }

        public async Task<Result<BossResponse?>> GetByIdAsync(Guid id)
        {
            var departmentHead = await _departmentHeadRepository.GetByIdAsync(id);
            if (departmentHead == null)
            {
                return Result<BossResponse?>.Failure("Jefe de Departamento no encontrado.");
            }

            var response = new BossResponse
            {
                Id = departmentHead.Id,
                Name = departmentHead.Name!,
                EmploymentStatus = departmentHead.EmploymentStatus!,
                Identification = departmentHead.Identification
            };

            return Result<BossResponse?>.Success(response);
        }

        public async Task<Result<IEnumerable<DepartmentHead>>> GetAllAsync()
        {
            var departmentHeads = await _departmentHeadRepository.GetAllAsync();
            return Result<IEnumerable<DepartmentHead>>.Success(departmentHeads);
        }

        public async Task<Result<BossResponse>> CreateAsync(BossDto dto, Guid departmentId)
        {
            var departmentHead = new DepartmentHead(
                Guid.NewGuid(),
                dto.Name,
                dto.EmploymentStatus,
                dto.Identification,
                departmentId);

            await _departmentHeadRepository.AddAsync(departmentHead);

            var response = new BossResponse
            {
                Id = departmentHead.Id,
                Name = departmentHead.Name!,
                EmploymentStatus = departmentHead.EmploymentStatus!,
                Identification = departmentHead.Identification
            };

            return Result<BossResponse>.Success(response);
        }

        public async Task<Result<BossResponse>> UpdateAsync(Guid id, BossDto dto)
        {
            var departmentHead = await _departmentHeadRepository.GetByIdAsync(id);
            if (departmentHead == null)
            {
                return Result<BossResponse>.Failure("Jefe de Departamento no encontrado.");
            }

            await _departmentHeadRepository.UpdateAsync(departmentHead);

            var response = new BossResponse
            {
                Id = departmentHead.Id,
                Name = departmentHead.Name!,
                EmploymentStatus = departmentHead.EmploymentStatus!,
                Identification = departmentHead.Identification
            };

            return Result<BossResponse>.Success(response);
        }

        public async Task<Result<string>> DeleteAsync(Guid id)
        {
            var departmentHead = await _departmentHeadRepository.GetByIdAsync(id);
            if (departmentHead == null)
            {
                return Result<string>.Failure("Jefe de Departamento no encontrado.");
            }
            await _departmentHeadRepository.DeleteAsync(departmentHead);
            return Result<string>.Success("Jefe de Departamento eliminado exitosamente.");
        }
    }
}