using PolyclinicDomain.IRepositories;
using PolyclinicDomain.Entities;
using Application.DTOs.Request;
using Application.DTOs.Response;
using PolyclinicApplication.Common.Results;
using Application.Services.Interfaces;
using System.Linq;

namespace Application.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public DoctorService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<DoctorResponseDto>> GetByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id) as Doctor;
            if (employee == null)
                return Result<DoctorResponseDto>.Failure("Doctor no encontrado");

            return Result<DoctorResponseDto>.Success(MapToResponse(employee));
        }

        public async Task<Result<IEnumerable<DoctorResponseDto>>> GetAllAsync()
        {
            var employees = (await _employeeRepository.GetAllAsync()).OfType<Doctor>();
            var doctorList = employees.Select(MapToResponse);
            return Result<IEnumerable<DoctorResponseDto>>.Success(doctorList);
        }

        public async Task<Result<DoctorResponseDto>> CreateAsync(DoctorDto dto)
        {
            var employee = new Doctor(
                Guid.NewGuid(),
                dto.Identification,
                dto.Name,
                dto.EmploymentStatus,
                dto.DepartmentId
            );

            await _employeeRepository.AddAsync(employee);

            return Result<DoctorResponseDto>.Success(MapToResponse(employee));
        }

        public async Task<Result<DoctorResponseDto>> UpdateAsync(Guid id, DoctorDto dto)
        {
            var entity = await _employeeRepository.GetByIdAsync(id) as Doctor;
            if (entity == null)
                return Result<DoctorResponseDto>.Failure("Doctor no encontrado");

            // Crear nueva instancia para respetar propiedades de solo lectura
            var updatedEntity = new Doctor(
                entity.Id,
                dto.Identification ?? entity.Identification,
                dto.Name ?? entity.Name,
                dto.EmploymentStatus ?? entity.EmploymentStatus,
                dto.DepartmentId
            );

            await _employeeRepository.UpdateAsync(updatedEntity);

            return Result<DoctorResponseDto>.Success(MapToResponse(updatedEntity));
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id) as Doctor;
            if (employee == null)
                return Result<bool>.Failure("Doctor no encontrado");

            await _employeeRepository.DeleteAsync(employee);

            return Result<bool>.Success(true);
        }

        private DoctorResponseDto MapToResponse(Doctor employee)
        {
            return new DoctorResponseDto
            {
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name
            };
        }
    }
}
