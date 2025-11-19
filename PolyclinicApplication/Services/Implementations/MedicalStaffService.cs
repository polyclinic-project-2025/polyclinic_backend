using PolyclinicDomain.IRepositories;
using PolyclinicDomain.Entities;
using Application.DTOs.Request;
using Application.DTOs.Response;
using PolyclinicApplication.Common.Results;
using Application.Services.Interfaces;
using System.Linq;

namespace Application.Services.Implementations
{
    public class MedicalStaffService : IMedicalStaffService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public MedicalStaffService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<MedicalStaffResponseDto>> GetByIdAsync(Guid id)
        {
            // Cast seguro a MedicalStaff
            var employee = await _employeeRepository.GetByIdAsync(id) as MedicalStaff;
            if (employee == null)
                return Result<MedicalStaffResponseDto>.Failure("Empleado médico no encontrado");

            return Result<MedicalStaffResponseDto>.Success(MapToResponse(employee));
        }

        public async Task<Result<IEnumerable<MedicalStaffResponseDto>>> GetAllAsync()
        {
            // Filtrar solo MedicalStaff
            var employees = (await _employeeRepository.GetAllAsync()).OfType<MedicalStaff>();
            var medicalStaffList = employees.Select(MapToResponse);
            return Result<IEnumerable<MedicalStaffResponseDto>>.Success(medicalStaffList);
        }

        public async Task<Result<MedicalStaffResponseDto>> CreateAsync(MedicalStaffDto dto)
        {
            var employee = new MedicalStaff(
                Guid.NewGuid(),
                dto.Identification,
                dto.Name,
                dto.EmploymentStatus,
                dto.DepartmentId
            );

            await _employeeRepository.AddAsync(employee);

            return Result<MedicalStaffResponseDto>.Success(MapToResponse(employee));
        }

        public async Task<Result<MedicalStaffResponseDto>> UpdateAsync(Guid id, MedicalStaffDto dto)
        {
            var entity = await _employeeRepository.GetByIdAsync(id) as MedicalStaff;
            if (entity == null)
                return Result<MedicalStaffResponseDto>.Failure("Personal médico no encontrado");

            // Crear nueva instancia para propiedades de solo lectura
            var updatedEntity = new MedicalStaff(
                entity.Id,
                dto.Identification,
                dto.Name,
                dto.EmploymentStatus,
                dto.DepartmentId
            );

            await _employeeRepository.UpdateAsync(updatedEntity);

            return Result<MedicalStaffResponseDto>.Success(MapToResponse(updatedEntity));
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id) as MedicalStaff;
            if (employee == null)
                return Result<bool>.Failure("Empleado médico no encontrado");

            await _employeeRepository.DeleteAsync(employee);

            return Result<bool>.Success(true);
        }

        private MedicalStaffResponseDto MapToResponse(MedicalStaff employee)
        {
            return new MedicalStaffResponseDto(
                DepartmentId: employee.DepartmentId,
                DepartmentName: employee.Department?.Name
            );
        }
    }
}
