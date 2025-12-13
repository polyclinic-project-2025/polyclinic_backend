using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.Services.Interfaces;
using PolyclinicApplication.DTOs.Departments;
using PolyclinicDomain.Entities;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;

        public DepartmentService(
            IDepartmentRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        public async Task<Result<DepartmentDto>> CreateAsync(CreateDepartmentDto dto)
        {
            try
            {
                // Verificar si ya existe un departamento con ese nombre
                var exists = await _repository.ExistsByNameAsync(dto.Name);
                if (exists)
                    return Result<DepartmentDto>.Failure($"Ya existe un departamento con el nombre '{dto.Name}'.");

                // Crear entidad
                var department = new Department(Guid.NewGuid(), dto.Name);

                // Guardar en BD
                await _repository.AddAsync(department);

                var departmentDto = _mapper.Map<DepartmentDto>(department);
                return Result<DepartmentDto>.Success(departmentDto);
            }
            catch (Exception ex)
            {
                return Result<DepartmentDto>.Failure($"Error al guardar el departamento: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<DepartmentDto>>> GetAllAsync()
        {
            try
            {
                var entities = await _repository.GetAllAsync();
                var entitiesDtos = _mapper.Map<IEnumerable<DepartmentDto>>(entities);
                return Result<IEnumerable<DepartmentDto>>.Success(entitiesDtos);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<DepartmentDto>>.Failure($"Error al obtener departamentos: {ex.Message}");
            }
        }

        public async Task<Result<DepartmentDto?>> GetByIdAsync(Guid id)
        {
            try
            {
                var department = await _repository.GetWithHeadAsync(id);
                if (department == null)
                    return Result<DepartmentDto?>.Failure("Departamento no encontrado.");

                var departmentDto = _mapper.Map<DepartmentDto>(department);
                return Result<DepartmentDto?>.Success(departmentDto);
            }
            catch (Exception ex)
            {
                return Result<DepartmentDto?>.Failure($"Error al obtener departamento: {ex.Message}");
            }
        }

        public async Task<Result<bool>> UpdateAsync(Guid id, UpdateDepartmentDto dto)
        {
            try
            {
                // Obtener departamento
                var department = await _repository.GetByIdAsync(id);
                if (department == null)
                    return Result<bool>.Failure("Departamento no encontrado.");

                // Actualizar nombre si cambió
                if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != department.Name)
                {
                    var exists = await _repository.ExistsByNameAsync(dto.Name);
                    if (exists)
                        return Result<bool>.Failure($"Ya existe un departamento con el nombre '{dto.Name}'.");

                    department.ChangeName(dto.Name);
                }

                // Guardar cambios
                await _repository.UpdateAsync(department);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error al actualizar el departamento: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            try
            {
                // Verificar que existe
                var department = await _repository.GetByIdAsync(id);
                if (department == null)
                    return Result<bool>.Failure("Departamento no encontrado.");

                // Eliminar
                await _repository.DeleteByIdAsync(id);
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error al eliminar el departamento: {ex.Message}");
            }
        }


        public async Task<Result<List<Doctor>>> GetDoctorsByDepartmentIdAsync(Guid departmentId)
        {
            try
            {
                var doctors = await _repository.GetDoctorsByDepartmentIdAsync(departmentId);
                return Result<List<Doctor>>.Success(doctors);
            }
            catch (Exception ex)
            {
                return Result<List<Doctor>>.Failure($"Error al obtener doctores del departamento: {ex.Message}");
            }
        }
    }
}
