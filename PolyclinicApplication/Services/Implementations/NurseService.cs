using PolyclinicDomain.IRepositories;
using PolyclinicDomain.Entities;
using Application.DTOs.Request;
using Application.DTOs.Response;
using PolyclinicApplication.Common.Results;
using Application.Services.Interfaces;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class NurseService : INurseService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public NurseService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<NurseResponseDto>> GetByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id) as Nurse;
            if (employee == null)
                return Result<NurseResponseDto>.Failure("Enfermera/o no encontrada/o");

            return Result<NurseResponseDto>.Success(MapToResponse(employee));
        }

        public async Task<Result<IEnumerable<NurseResponseDto>>> GetAllAsync()
        {
            var employees = (await _employeeRepository.GetAllAsync()).OfType<Nurse>();
            var nurseList = employees.Select(MapToResponse);
            return Result<IEnumerable<NurseResponseDto>>.Success(nurseList);
        }

        public async Task<Result<NurseResponseDto>> CreateAsync(NurseDto dto)
        {
            var employee = new Nurse(
                Guid.NewGuid(),
                dto.Identification,
                dto.Name,
                dto.EmploymentStatus,
                dto.NursingId
            );

            await _employeeRepository.AddAsync(employee);

            return Result<NurseResponseDto>.Success(MapToResponse(employee));
        }

        public async Task<Result<NurseResponseDto>> UpdateAsync(Guid id, NurseDto dto)
        {
            var entity = await _employeeRepository.GetByIdAsync(id) as Nurse;
            if (entity == null)
                return Result<NurseResponseDto>.Failure("Enfermera/o no encontrada/o");

            var updatedEntity = new Nurse(
                entity.Id,
                dto.Identification ?? entity.Identification,
                dto.Name ?? entity.Name,
                dto.EmploymentStatus ?? entity.EmploymentStatus,
                dto.NursingId
            );

            await _employeeRepository.UpdateAsync(updatedEntity);

            return Result<NurseResponseDto>.Success(MapToResponse(updatedEntity));
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id) as Nurse;
            if (employee == null)
                return Result<bool>.Failure("Enfermera/o no encontrada/o");

            await _employeeRepository.DeleteAsync(employee);

            return Result<bool>.Success(true);
        }

        private NurseResponseDto MapToResponse(Nurse employee)
        {
            return new NurseResponseDto(
                employee.NursingId,
                employee.Nursing?.Name,
                employee.UserId
            );
        }
    }
}
