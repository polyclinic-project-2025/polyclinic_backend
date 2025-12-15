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
    public class DepartmentHeadServiceTests
    {
        private readonly Mock<IDepartmentHeadRepository> _mockRepository;
        private readonly Mock<IDepartmentRepository> _mockDepartmentRepository;
        private readonly Mock<IDoctorRepository> _mockDoctorRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly DepartmentHeadService _service;

        public DepartmentHeadServiceTests()
        {
            _mockRepository = new Mock<IDepartmentHeadRepository>();
            _mockDepartmentRepository = new Mock<IDepartmentRepository>();
            _mockDoctorRepository = new Mock<IDoctorRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new DepartmentHeadService(
                _mockRepository.Object,
                _mockMapper.Object,
                _mockDepartmentRepository.Object,
                _mockDoctorRepository.Object
            );
        }

        #region GetAllDepartmentHeadAsync Tests

        [Fact]
        public async Task GetAllDepartmentHeadAsync_DepartmentHeadsExist_ReturnsSuccessWithList()
        {
            // ARRANGE
            var departmentHeads = new List<DepartmentHead>
            {
                new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow),
                new DepartmentHead(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow)
            };

            var departmentHeadResponses = new List<DepartmentHeadResponse>
            {
                new DepartmentHeadResponse 
                { 
                    DepartmentHeadId = departmentHeads[0].DepartmentHeadId,
                    DoctorId = departmentHeads[0].DoctorId,
                    DepartmentId = departmentHeads[0].DepartmentId,
                    AssignedAt = departmentHeads[0].AssignedAt
                },
                new DepartmentHeadResponse 
                { 
                    DepartmentHeadId = departmentHeads[1].DepartmentHeadId,
                    DoctorId = departmentHeads[1].DoctorId,
                    DepartmentId = departmentHeads[1].DepartmentId,
                    AssignedAt = departmentHeads[1].AssignedAt
                }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(departmentHeads);
            _mockMapper.Setup(m => m.Map<IEnumerable<DepartmentHeadResponse>>(departmentHeads))
                .Returns(departmentHeadResponses);

            // ACT
            var result = await _service.GetAllDepartmentHeadAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllDepartmentHeadAsync_NoDepartmentHeads_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var emptyList = new List<DepartmentHead>();
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<IEnumerable<DepartmentHeadResponse>>(emptyList))
                .Returns(new List<DepartmentHeadResponse>());

            // ACT
            var result = await _service.GetAllDepartmentHeadAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllDepartmentHeadAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            _mockRepository.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.GetAllDepartmentHeadAsync();

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion

        #region GetDepartmentHeadByIdAsync Tests

        [Fact]
        public async Task GetDepartmentHeadByIdAsync_ValidId_ReturnsSuccessWithDepartmentHead()
        {
            // ARRANGE
            var departmentHeadId = Guid.NewGuid();
            var departmentHead = new DepartmentHead(
                departmentHeadId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow
            );
            var response = new DepartmentHeadResponse 
            { 
                DepartmentHeadId = departmentHeadId,
                DoctorId = departmentHead.DoctorId,
                DepartmentId = departmentHead.DepartmentId,
                AssignedAt = departmentHead.AssignedAt
            };

            _mockRepository.Setup(r => r.GetByIdAsync(departmentHeadId)).ReturnsAsync(departmentHead);
            _mockMapper.Setup(m => m.Map<DepartmentHeadResponse>(departmentHead)).Returns(response);

            // ACT
            var result = await _service.GetDepartmentHeadByIdAsync(departmentHeadId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(departmentHeadId, result.Value.DepartmentHeadId);
        }

        [Fact]
        public async Task GetDepartmentHeadByIdAsync_DepartmentHeadNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentHeadId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(departmentHeadId)).ReturnsAsync((DepartmentHead?)null);

            // ACT
            var result = await _service.GetDepartmentHeadByIdAsync(departmentHeadId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("no encontrado", result.ErrorMessage);
        }

        [Fact]
        public async Task GetDepartmentHeadByIdAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentHeadId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(departmentHeadId))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.GetDepartmentHeadByIdAsync(departmentHeadId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion

        #region GetDepartmentHeadByDepartmentIdAsync Tests

        [Fact]
        public async Task GetDepartmentHeadByDepartmentIdAsync_ValidDepartmentId_ReturnsSuccessWithDepartmentHead()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var department = new Department(departmentId, "Cardiology");
            var departmentHead = new DepartmentHead(
                Guid.NewGuid(),
                Guid.NewGuid(),
                departmentId,
                DateTime.UtcNow
            );
            var response = new DepartmentHeadResponse 
            { 
                DepartmentHeadId = departmentHead.DepartmentHeadId,
                DoctorId = departmentHead.DoctorId,
                DepartmentId = departmentHead.DepartmentId,
                AssignedAt = departmentHead.AssignedAt
            };

            _mockDepartmentRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(department);
            _mockRepository.Setup(r => r.GetByDepartmentIdAsync(departmentId)).ReturnsAsync(departmentHead);
            _mockMapper.Setup(m => m.Map<DepartmentHeadResponse>(departmentHead)).Returns(response);

            // ACT
            var result = await _service.GetDepartmentHeadByDepartmentIdAsync(departmentId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(departmentHead.DepartmentHeadId, result.Value.DepartmentHeadId);
        }

        [Fact]
        public async Task GetDepartmentHeadByDepartmentIdAsync_DepartmentNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            _mockDepartmentRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync((Department?)null);

            // ACT
            var result = await _service.GetDepartmentHeadByDepartmentIdAsync(departmentId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Departamento no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.GetByDepartmentIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetDepartmentHeadByDepartmentIdAsync_DepartmentHeadNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            var department = new Department(departmentId, "Cardiology");

            _mockDepartmentRepository.Setup(r => r.GetByIdAsync(departmentId)).ReturnsAsync(department);
            _mockRepository.Setup(r => r.GetByDepartmentIdAsync(departmentId)).ReturnsAsync((DepartmentHead?)null);

            // ACT
            var result = await _service.GetDepartmentHeadByDepartmentIdAsync(departmentId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Jefe de departamento no encontrado", result.ErrorMessage);
        }

        [Fact]
        public async Task GetDepartmentHeadByDepartmentIdAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentId = Guid.NewGuid();
            _mockDepartmentRepository.Setup(r => r.GetByIdAsync(departmentId))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.GetDepartmentHeadByDepartmentIdAsync(departmentId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion

        #region AssignDepartmentHeadAsync Tests

        [Fact]
        public async Task AssignDepartmentHeadAsync_ValidRequest_ReturnsSuccessResult()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var request = new AssignDepartmentHeadRequest { DoctorId = doctorId };
            var doctor = new Doctor(doctorId, "12345678", "Dr. Juan Perez", "Active", departmentId);
            var response = new DepartmentHeadResponse 
            { 
                DepartmentHeadId = Guid.NewGuid(),
                DoctorId = doctorId,
                DepartmentId = departmentId,
                AssignedAt = DateTime.UtcNow
            };

            _mockDoctorRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(doctor);
            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync((DepartmentHead?)null);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<DepartmentHead>()))
                .ReturnsAsync(It.IsAny<DepartmentHead>());
            _mockMapper.Setup(m => m.Map<DepartmentHeadResponse>(It.IsAny<DepartmentHead>()))
                .Returns(response);

            // ACT
            var result = await _service.AssignDepartmentHeadAsync(request);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<DepartmentHead>()), Times.Once);
        }

        [Fact]
        public async Task AssignDepartmentHeadAsync_DoctorNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var request = new AssignDepartmentHeadRequest { DoctorId = Guid.NewGuid() };
            _mockDoctorRepository.Setup(r => r.GetByIdAsync(request.DoctorId)).ReturnsAsync((Doctor?)null);

            // ACT
            var result = await _service.AssignDepartmentHeadAsync(request);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Doctor no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<DepartmentHead>()), Times.Never);
        }

        [Fact]
        public async Task AssignDepartmentHeadAsync_DoctorAlreadyDepartmentHead_ReturnsFailureResult()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var request = new AssignDepartmentHeadRequest { DoctorId = doctorId };
            var doctor = new Doctor(doctorId, "12345678", "Dr. Juan Perez", "Active", departmentId);
            var existingDepartmentHead = new DepartmentHead(
                Guid.NewGuid(),
                doctorId,
                departmentId,
                DateTime.UtcNow
            );

            _mockDoctorRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(doctor);
            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(existingDepartmentHead);

            // ACT
            var result = await _service.AssignDepartmentHeadAsync(request);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("ya es jefe de departamento", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<DepartmentHead>()), Times.Never);
        }

        [Fact]
        public async Task AssignDepartmentHeadAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var doctorId = Guid.NewGuid();
            var departmentId = Guid.NewGuid();
            var request = new AssignDepartmentHeadRequest { DoctorId = doctorId };
            var doctor = new Doctor(doctorId, "12345678", "Dr. Juan Perez", "Active", departmentId);

            _mockDoctorRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync(doctor);
            _mockRepository.Setup(r => r.GetByIdAsync(doctorId)).ReturnsAsync((DepartmentHead?)null);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<DepartmentHead>()))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.AssignDepartmentHeadAsync(request);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion

        #region RemoveDepartmentHeadAsync Tests

        [Fact]
        public async Task RemoveDepartmentHeadAsync_ValidId_ReturnsSuccessResult()
        {
            // ARRANGE
            var departmentHeadId = Guid.NewGuid();
            var departmentHead = new DepartmentHead(
                departmentHeadId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow
            );

            _mockRepository.Setup(r => r.GetByIdAsync(departmentHeadId)).ReturnsAsync(departmentHead);
            _mockRepository.Setup(r => r.DeleteAsync(departmentHead)).Returns(Task.CompletedTask);

            // ACT
            var result = await _service.RemoveDepartmentHeadAsync(departmentHeadId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.DeleteAsync(departmentHead), Times.Once);
        }

        [Fact]
        public async Task RemoveDepartmentHeadAsync_DepartmentHeadNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentHeadId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(departmentHeadId)).ReturnsAsync((DepartmentHead?)null);

            // ACT
            var result = await _service.RemoveDepartmentHeadAsync(departmentHeadId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<DepartmentHead>()), Times.Never);
        }

        [Fact]
        public async Task RemoveDepartmentHeadAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentHeadId = Guid.NewGuid();
            var departmentHead = new DepartmentHead(
                departmentHeadId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.UtcNow
            );

            _mockRepository.Setup(r => r.GetByIdAsync(departmentHeadId)).ReturnsAsync(departmentHead);
            _mockRepository.Setup(r => r.DeleteAsync(departmentHead))
                .ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.RemoveDepartmentHeadAsync(departmentHeadId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        #endregion
    }
}