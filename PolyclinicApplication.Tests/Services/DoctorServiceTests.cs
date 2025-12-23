using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Moq;
using AutoMapper;
using PolyclinicApplication.Services.Implementations;
using PolyclinicApplication.DTOs.Request;
using PolyclinicApplication.DTOs.Response;
using PolyclinicDomain.Entities;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.Common.Results;

namespace PolyclinicApplication.Tests.Services
{
    public class DoctorServiceTests
    {
        private readonly Mock<IDoctorRepository> _mockRepository;
        private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DoctorService _service;

        public DoctorServiceTests()
        {
            _mockRepository = new Mock<IDoctorRepository>();
            _mockDepartmentRepository = new Mock<IDepartmentRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new DoctorService(
                _mockRepository.Object,
                _mockMapper.Object,
                _mockDepartmentRepository.Object
            );
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ValidDoctor_ReturnsSuccessResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var createDto = new CreateDoctorRequest
            {
                Identification = "12345678",
                Name = "Dr. Juan Perez",
                EmploymentStatus = "Active",
                DepartmentId = departmentId
            };
            var department = new Department(departmentId, "Cardiology");
            var doctor = new Doctor(
                Guid.NewGuid(),
                createDto.Identification,
                createDto.Name,
                createDto.EmploymentStatus,
                departmentId
            );
            var doctorResponse = new DoctorResponse
            {
                EmployeeId = doctor.EmployeeId,
                Name = doctor.Name,
                Identification = doctor.Identification,
                EmploymentStatus = doctor.EmploymentStatus,
                DepartmentId = doctor.DepartmentId
            };

            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(createDto.Identification))
                .ReturnsAsync(false);
            _mockDepartmentRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(department);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Doctor>())).ReturnsAsync(doctor);
            _mockMapper.Setup(m => m.Map<DoctorResponse>(It.IsAny<Doctor>())).Returns(doctorResponse);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(createDto.Name, result.Value.Name);
            Assert.Equal(createDto.Identification, result.Value.Identification);
            Assert.Equal(departmentId, result.Value.DepartmentId);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Doctor>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DuplicateIdentification_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreateDoctorRequest
            {
                Identification = "12345678",
                Name = "Dr. Juan Perez",
                EmploymentStatus = "Active",
                DepartmentId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(createDto.Identification))
                .ReturnsAsync(true);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Ya existe", result.ErrorMessage);
            Assert.Contains("identificación", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Doctor>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DepartmentNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var createDto = new CreateDoctorRequest
            {
                Identification = "12345678",
                Name = "Dr. Juan Perez",
                EmploymentStatus = "Active",
                DepartmentId = departmentId
            };

            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(createDto.Identification))
                .ReturnsAsync(false);
            _mockDepartmentRepository.Setup(r => r.GetByIdAsync(departmentId))
                .ReturnsAsync((Department?)null);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Departamento no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Doctor>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var createDto = new CreateDoctorRequest
            {
                Identification = "12345678",
                Name = "Dr. Juan Perez",
                EmploymentStatus = "Active",
                DepartmentId = departmentId
            };
            var department = new Department(departmentId, "Cardiology");

            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(createDto.Identification))
                .ReturnsAsync(false);
            _mockDepartmentRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(department);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Doctor>()))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_DoctorsExist_ReturnsSuccessWithList()
        {
            // ARRANGE
            var doctors = new List<Doctor>
            {
                new Doctor(Guid.NewGuid(), "12345678", "Dr. Juan Perez", "Active", Guid.NewGuid()),
                new Doctor(Guid.NewGuid(), "87654321", "Dr. Maria Garcia", "Active", Guid.NewGuid())
            };

            var doctorResponses = new List<DoctorResponse>
            {
                new DoctorResponse 
                { 
                    EmployeeId = doctors[0].EmployeeId, 
                    Name = "Dr. Juan Perez",
                    Identification = doctors[0].Identification,
                    EmploymentStatus = doctors[0].EmploymentStatus,
                    DepartmentId = doctors[0].DepartmentId
                },
                new DoctorResponse 
                { 
                    EmployeeId = doctors[1].EmployeeId, 
                    Name = "Dr. Maria Garcia",
                    Identification = doctors[1].Identification,
                    EmploymentStatus = doctors[1].EmploymentStatus,
                    DepartmentId = doctors[1].DepartmentId
                }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(doctors);
            _mockMapper.Setup(m => m.Map<IEnumerable<DoctorResponse>>(doctors)).Returns(doctorResponses);

            // ACT
            var result = await _service.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetAllAsync_NoDoctors_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var emptyList = new List<Doctor>();
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<IEnumerable<DoctorResponse>>(emptyList))
                .Returns(new List<DoctorResponse>());

            // ACT
            var result = await _service.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsSuccessWithDoctor()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var doctor = new Doctor(doctorId, "12345678", "Dr. Juan Perez", "Active", departmentId);
            var doctorResponse = new DoctorResponse
            {
                EmployeeId = doctorId,
                Name = "Dr. Juan Perez",
                Identification = "12345678",
                EmploymentStatus = "Active",
                DepartmentId = departmentId
            };

            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(doctor);
            _mockMapper.Setup(m => m.Map<DoctorResponse>(doctor)).Returns(doctorResponse);

            // ACT
            var result = await _service.GetByIdAsync(doctorId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(doctorId, result.Value.EmployeeId);
            Assert.Equal("Dr. Juan Perez", result.Value.Name);
        }

        [Fact]
        public async Task GetByIdAsync_DoctorNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync((Doctor?)null);

            // ACT
            var result = await _service.GetByIdAsync(doctorId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("no encontrado", result.ErrorMessage);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ValidUpdate_ReturnsSuccessResult()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            var existingDoctor = new Doctor(doctorId, "12345678", "Dr. Juan Perez", "Active", Guid.NewGuid());
            var updateDto = new UpdateDoctorRequest
            {
                Name = "Dr. Juan Updated",
                Identification = "12345678",
                EmploymentStatus = "Inactive",
                DepartmentId = null
            };

            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(existingDoctor);
            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(updateDto.Identification))
                .ReturnsAsync(false);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Doctor>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _service.UpdateAsync(doctorId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Doctor>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DoctorNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            var updateDto = new UpdateDoctorRequest { Name = "Dr. Juan Updated" };
            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync((Doctor?)null);

            // ACT
            var result = await _service.UpdateAsync(doctorId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Doctor>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DuplicateIdentification_ReturnsFailureResult()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            var existingDoctor = new Doctor(doctorId, "12345678", "Dr. Juan Perez", "Active", Guid.NewGuid());
            var updateDto = new UpdateDoctorRequest
            {
                Identification = "87654321" // Different identification
            };

            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(existingDoctor);
            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(updateDto.Identification))
                .ReturnsAsync(true);

            // ACT
            var result = await _service.UpdateAsync(doctorId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Ya existe", result.ErrorMessage);
            Assert.Contains("identificación", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Doctor>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_SameIdentification_ReturnsSuccessResult()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            var existingDoctor = new Doctor(doctorId, "12345678", "Dr. Juan Perez", "Active", Guid.NewGuid());
            var updateDto = new UpdateDoctorRequest
            {
                Identification = "12345678", // Same identification
                Name = "Dr. Juan Updated"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(existingDoctor);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Doctor>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _service.UpdateAsync(doctorId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.ExistsByIdentificationAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_UpdateName_ReturnsSuccessResult()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            var existingDoctor = new Doctor(doctorId, "12345678", "Dr. Juan Perez", "Active", Guid.NewGuid());
            var updateDto = new UpdateDoctorRequest { Name = "Dr. Juan Updated" };

            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(existingDoctor);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Doctor>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _service.UpdateAsync(doctorId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Doctor>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_UpdateEmploymentStatus_ReturnsSuccessResult()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            var existingDoctor = new Doctor(doctorId, "12345678", "Dr. Juan Perez", "Active", Guid.NewGuid());
            var updateDto = new UpdateDoctorRequest { EmploymentStatus = "Inactive" };

            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(existingDoctor);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Doctor>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _service.UpdateAsync(doctorId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Doctor>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_UpdateDepartmentId_ReturnsSuccessResult()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            var newDepartmentId = Guid.NewGuid();
            var existingDoctor = new Doctor(doctorId, "12345678", "Dr. Juan Perez", "Active", Guid.NewGuid());
            var updateDto = new UpdateDoctorRequest { DepartmentId = newDepartmentId };

            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(existingDoctor);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Doctor>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _service.UpdateAsync(doctorId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Doctor>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            var existingDoctor = new Doctor(doctorId, "12345678", "Dr. Juan Perez", "Active", Guid.NewGuid());
            var updateDto = new UpdateDoctorRequest { Name = "Dr. Juan Updated" };

            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(existingDoctor);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Doctor>()))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.UpdateAsync(doctorId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ValidId_ReturnsSuccessResult()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            var doctor = new Doctor(doctorId, "12345678", "Dr. Juan Perez", "Active", Guid.NewGuid());

            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(doctor);
            _mockRepository.Setup(r => r.DeleteAsync(doctor)).Returns(Task.CompletedTask);

            // ACT
            var result = await _service.DeleteAsync(doctorId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.DeleteAsync(doctor), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DoctorNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync((Doctor?)null);

            // ACT
            var result = await _service.DeleteAsync(doctorId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Doctor>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            var doctor = new Doctor(doctorId, "12345678", "Dr. Juan Perez", "Active", Guid.NewGuid());

            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(doctor);
            _mockRepository.Setup(r => r.DeleteAsync(doctor))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.DeleteAsync(doctorId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion
    }
}