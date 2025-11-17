using PolyclinicDomain.IRepositories;
using PolyclinicDomain.Entities;
using Application.DTOs.Request;
using Application.DTOs.Response;
using PolyclinicApplication.Common.Results;
using Application.Services.Interfaces;
using System.Linq;

namespace Application.Services.Implementations
{
    public class NursingHeadService : INursingHeadService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public NursingHeadService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<NursingHeadResponseDto>> GetByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id) as NursingHead;
            if (employee == null)
                return Result<NursingHeadResponseDto>.Failure("Jefe de enfermería no encontrado");

            return Result<NursingHeadResponseDto>.Success(MapToResponse(employee));
        }

        public async Task<Result<IEnumerable<NursingHeadResponseDto>>> GetAllAsync()
        {
            var employees = (await _employeeRepository.GetAllAsync()).OfType<NursingHead>();
            var headsList = employees.Select(MapToResponse);
            return Result<IEnumerable<NursingHeadResponseDto>>.Success(headsList);
        }

        public async Task<Result<NursingHeadResponseDto>> CreateAsync(NursingHeadDto dto)
        {
            var employee = new NursingHead(
                Guid.NewGuid(),
                dto.Identification,
                dto.Name,
                dto.EmploymentStatus,
                dto.ManagedNursingId
            );

            await _employeeRepository.AddAsync(employee);

            return Result<NursingHeadResponseDto>.Success(MapToResponse(employee));
        }

        public async Task<Result<NursingHeadResponseDto>> UpdateAsync(Guid id, NursingHeadDto dto)
        {
            var entity = await _employeeRepository.GetByIdAsync(id) as NursingHead;
            if (entity == null)
                return Result<NursingHeadResponseDto>.Failure("Jefe de enfermería no encontrado");

            var updatedEntity = new NursingHead(
                entity.Id,
                dto.Identification ?? entity.Identification,
                dto.Name ?? entity.Name,
                dto.EmploymentStatus ?? entity.EmploymentStatus,
                dto.ManagedNursingId
            );

            await _employeeRepository.UpdateAsync(updatedEntity);

            return Result<NursingHeadResponseDto>.Success(MapToResponse(updatedEntity));
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id) as NursingHead;
            if (employee == null)
                return Result<bool>.Failure("Jefe de enfermería no encontrado");

            await _employeeRepository.DeleteAsync(employee);

            return Result<bool>.Success(true);
        }

        private NursingHeadResponseDto MapToResponse(NursingHead employee)
        {
            return new NursingHeadResponseDto(
                ManagedNursingId: employee.ManagedNursingId,
                ManagedNursingName: employee.ManagedNursing?.Name
            );
        }
    }
}
