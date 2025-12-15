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
    public class ConsultationDerivationServiceTests
    {
        private readonly Mock<IConsultationDerivationRepository> _mockRepository;
        private readonly Mock<IDerivationRepository> _mockDerivationRepo;
        private readonly Mock<IDoctorRepository> _mockDoctorRepo;
        private readonly Mock<IDepartmentHeadRepository> _mockDepartmentHeadRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ConsultationDerivationService _service;

        public ConsultationDerivationServiceTests()
        {
            _mockRepository = new Mock<IConsultationDerivationRepository>();
            _mockDerivationRepo = new Mock<IDerivationRepository>();
            _mockDoctorRepo = new Mock<IDoctorRepository>();
            _mockDepartmentHeadRepo = new Mock<IDepartmentHeadRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new ConsultationDerivationService(
                _mockRepository.Object,
                _mockDerivationRepo.Object,
                _mockDoctorRepo.Object,
                _mockDepartmentHeadRepo.Object,
                _mockMapper.Object
            );
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ValidConsultation_ReturnsSuccessResult()
        {
            // ARRANGE
            var departmentToId = Guid.NewGuid();
            var createDto = new CreateConsultationDerivationDto
            {
                Diagnosis = "Hipertensión arterial",
                DerivationId = Guid.NewGuid(),
                DateTimeCDer = DateTime.Now,
                DoctorId = Guid.NewGuid(),
                DepartmentHeadId = Guid.NewGuid()
            };

            var derivation = new Derivation(
                createDto.DerivationId,
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentToId
            );

            var doctor = new Doctor(
                createDto.DoctorId,
                "12345678",
                "Dr. Juan Pérez",
                "Active",
                departmentToId
            );

            var departmentHead = new DepartmentHead(
                createDto.DepartmentHeadId,
                Guid.NewGuid(),
                departmentToId,
                DateTime.Now
            );

            var consultation = new ConsultationDerivation(
                Guid.NewGuid(),
                createDto.Diagnosis,
                createDto.DerivationId,
                createDto.DateTimeCDer,
                createDto.DoctorId,
                createDto.DepartmentHeadId
            );

            var consultationDto = new ConsultationDerivationDto
            {
                ConsultationDerivationId = consultation.ConsultationDerivationId,
                Diagnosis = createDto.Diagnosis
            };

            _mockDerivationRepo.Setup(r => r.GetByIdAsync(createDto.DerivationId)).ReturnsAsync(derivation);
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(createDto.DoctorId)).ReturnsAsync(doctor);
            _mockDepartmentHeadRepo.Setup(r => r.GetByIdAsync(createDto.DepartmentHeadId)).ReturnsAsync(departmentHead);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<ConsultationDerivation>())).ReturnsAsync(consultation);
            _mockMapper.Setup(m => m.Map<ConsultationDerivationDto>(It.IsAny<ConsultationDerivation>())).Returns(consultationDto);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(createDto.Diagnosis, result.Value.Diagnosis);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<ConsultationDerivation>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DerivationNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreateConsultationDerivationDto
            {
                Diagnosis = "Test",
                DerivationId = Guid.NewGuid(),
                DateTimeCDer = DateTime.Now,
                DoctorId = Guid.NewGuid(),
                DepartmentHeadId = Guid.NewGuid()
            };

            _mockDerivationRepo.Setup(r => r.GetByIdAsync(createDto.DerivationId)).ReturnsAsync((Derivation?)null);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Derivation not found", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<ConsultationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DoctorNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreateConsultationDerivationDto
            {
                Diagnosis = "Test",
                DerivationId = Guid.NewGuid(),
                DateTimeCDer = DateTime.Now,
                DoctorId = Guid.NewGuid(),
                DepartmentHeadId = Guid.NewGuid()
            };

            var derivation = new Derivation(
                createDto.DerivationId,
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            _mockDerivationRepo.Setup(r => r.GetByIdAsync(createDto.DerivationId)).ReturnsAsync(derivation);
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(createDto.DoctorId)).ReturnsAsync((Doctor?)null);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Doctor not found", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<ConsultationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DepartmentHeadNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var createDto = new CreateConsultationDerivationDto
            {
                Diagnosis = "Test",
                DerivationId = Guid.NewGuid(),
                DateTimeCDer = DateTime.Now,
                DoctorId = Guid.NewGuid(),
                DepartmentHeadId = Guid.NewGuid()
            };

            var derivation = new Derivation(
                createDto.DerivationId,
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            var doctor = new Doctor(
                createDto.DoctorId,
                "12345678",
                "Dr. Test",
                "Active",
                Guid.NewGuid()
            );

            _mockDerivationRepo.Setup(r => r.GetByIdAsync(createDto.DerivationId)).ReturnsAsync(derivation);
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(createDto.DoctorId)).ReturnsAsync(doctor);
            _mockDepartmentHeadRepo.Setup(r => r.GetByIdAsync(createDto.DepartmentHeadId)).ReturnsAsync((DepartmentHead?)null);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("DepartmentHead not found", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<ConsultationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DepartmentHeadWrongDepartment_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentToId = Guid.NewGuid();
            var wrongDepartmentId = Guid.NewGuid();
            var createDto = new CreateConsultationDerivationDto
            {
                Diagnosis = "Test",
                DerivationId = Guid.NewGuid(),
                DateTimeCDer = DateTime.Now,
                DoctorId = Guid.NewGuid(),
                DepartmentHeadId = Guid.NewGuid()
            };

            var derivation = new Derivation(
                createDto.DerivationId,
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentToId
            );

            var doctor = new Doctor(
                createDto.DoctorId,
                "12345678",
                "Dr. Test",
                "Active",
                departmentToId
            );

            var departmentHead = new DepartmentHead(
                createDto.DepartmentHeadId,
                Guid.NewGuid(),
                wrongDepartmentId,
                DateTime.Now
            );

            _mockDerivationRepo.Setup(r => r.GetByIdAsync(createDto.DerivationId)).ReturnsAsync(derivation);
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(createDto.DoctorId)).ReturnsAsync(doctor);
            _mockDepartmentHeadRepo.Setup(r => r.GetByIdAsync(createDto.DepartmentHeadId)).ReturnsAsync(departmentHead);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("El jefe de departamento debe pertenecer al mismo departamento destino", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<ConsultationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_DoctorWrongDepartment_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentToId = Guid.NewGuid();
            var wrongDepartmentId = Guid.NewGuid();
            var createDto = new CreateConsultationDerivationDto
            {
                Diagnosis = "Test",
                DerivationId = Guid.NewGuid(),
                DateTimeCDer = DateTime.Now,
                DoctorId = Guid.NewGuid(),
                DepartmentHeadId = Guid.NewGuid()
            };

            var derivation = new Derivation(
                createDto.DerivationId,
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentToId
            );

            var doctor = new Doctor(
                createDto.DoctorId,
                "12345678",
                "Dr. Test",
                "Active",
                wrongDepartmentId
            );

            var departmentHead = new DepartmentHead(
                createDto.DepartmentHeadId,
                Guid.NewGuid(),
                departmentToId,
                DateTime.Now
            );

            _mockDerivationRepo.Setup(r => r.GetByIdAsync(createDto.DerivationId)).ReturnsAsync(derivation);
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(createDto.DoctorId)).ReturnsAsync(doctor);
            _mockDepartmentHeadRepo.Setup(r => r.GetByIdAsync(createDto.DepartmentHeadId)).ReturnsAsync(departmentHead);

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("El doctor tratante debe pertenecer al mismo departamento destino", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<ConsultationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var departmentToId = Guid.NewGuid();
            var createDto = new CreateConsultationDerivationDto
            {
                Diagnosis = "Test",
                DerivationId = Guid.NewGuid(),
                DateTimeCDer = DateTime.Now,
                DoctorId = Guid.NewGuid(),
                DepartmentHeadId = Guid.NewGuid()
            };

            var derivation = new Derivation(
                createDto.DerivationId,
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentToId
            );

            var doctor = new Doctor(
                createDto.DoctorId,
                "12345678",
                "Dr. Test",
                "Active",
                departmentToId
            );

            var departmentHead = new DepartmentHead(
                createDto.DepartmentHeadId,
                Guid.NewGuid(),
                departmentToId,
                DateTime.Now
            );

            _mockDerivationRepo.Setup(r => r.GetByIdAsync(createDto.DerivationId)).ReturnsAsync(derivation);
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(createDto.DoctorId)).ReturnsAsync(doctor);
            _mockDepartmentHeadRepo.Setup(r => r.GetByIdAsync(createDto.DepartmentHeadId)).ReturnsAsync(departmentHead);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<ConsultationDerivation>())).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.CreateAsync(createDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al guardar la consulta", result.ErrorMessage);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ValidUpdate_ReturnsSuccessResult()
        {
            // ARRANGE
            var consultationId = Guid.NewGuid();
            var departmentToId = Guid.NewGuid();
            var derivationId = Guid.NewGuid();

            var existingConsultation = new ConsultationDerivation(
                consultationId,
                "Old Diagnosis",
                derivationId,
                DateTime.Now,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            var updateDto = new UpdateConsultationDerivationDto
            {
                Diagnosis = "New Diagnosis",
                DateTimeCDer = DateTime.Now.AddDays(1),
                DoctorId = Guid.NewGuid(),
                DepartmentHeadId = Guid.NewGuid()
            };

            var derivation = new Derivation(
                derivationId,
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentToId
            );

            var doctor = new Doctor(
                updateDto.DoctorId,
                "12345678",
                "Dr. Test",
                "Active",
                departmentToId
            );

            var departmentHead = new DepartmentHead(
                updateDto.DepartmentHeadId,
                Guid.NewGuid(),
                departmentToId,
                DateTime.Now
            );

            _mockRepository.Setup(r => r.GetByIdAsync(consultationId)).ReturnsAsync(existingConsultation);
            _mockDerivationRepo.Setup(r => r.GetByIdAsync(derivationId)).ReturnsAsync(derivation);
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(updateDto.DoctorId)).ReturnsAsync(doctor);
            _mockDepartmentHeadRepo.Setup(r => r.GetByIdAsync(updateDto.DepartmentHeadId)).ReturnsAsync(departmentHead);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<ConsultationDerivation>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _service.UpdateAsync(consultationId, updateDto);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<ConsultationDerivation>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ConsultationNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var consultationId = Guid.NewGuid();
            var updateDto = new UpdateConsultationDerivationDto
            {
                Diagnosis = "New Diagnosis",
                DoctorId = Guid.NewGuid(),
                DepartmentHeadId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.GetByIdAsync(consultationId)).ReturnsAsync((ConsultationDerivation?)null);

            // ACT
            var result = await _service.UpdateAsync(consultationId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Record not found", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<ConsultationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DerivationNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var consultationId = Guid.NewGuid();
            var derivationId = Guid.NewGuid();

            var existingConsultation = new ConsultationDerivation(
                consultationId,
                "Diagnosis",
                derivationId,
                DateTime.Now,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            var updateDto = new UpdateConsultationDerivationDto
            {
                Diagnosis = "New Diagnosis",
                DoctorId = Guid.NewGuid(),
                DepartmentHeadId = Guid.NewGuid()
            };

            _mockRepository.Setup(r => r.GetByIdAsync(consultationId)).ReturnsAsync(existingConsultation);
            _mockDerivationRepo.Setup(r => r.GetByIdAsync(derivationId)).ReturnsAsync((Derivation?)null);

            // ACT
            var result = await _service.UpdateAsync(consultationId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Derivation not found", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<ConsultationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DoctorWrongDepartment_ReturnsFailureResult()
        {
            // ARRANGE
            var consultationId = Guid.NewGuid();
            var departmentToId = Guid.NewGuid();
            var wrongDepartmentId = Guid.NewGuid();
            var derivationId = Guid.NewGuid();

            var existingConsultation = new ConsultationDerivation(
                consultationId,
                "Diagnosis",
                derivationId,
                DateTime.Now,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            var updateDto = new UpdateConsultationDerivationDto
            {
                Diagnosis = "New Diagnosis",
                DoctorId = Guid.NewGuid(),
                DepartmentHeadId = Guid.NewGuid()
            };

            var derivation = new Derivation(
                derivationId,
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentToId
            );

            var doctor = new Doctor(
                updateDto.DoctorId,
                "12345678",
                "Dr. Test",
                "Active",
                wrongDepartmentId
            );

            var departmentHead = new DepartmentHead(
                updateDto.DepartmentHeadId,
                Guid.NewGuid(),
                departmentToId,
                DateTime.Now
            );

            _mockRepository.Setup(r => r.GetByIdAsync(consultationId)).ReturnsAsync(existingConsultation);
            _mockDerivationRepo.Setup(r => r.GetByIdAsync(derivationId)).ReturnsAsync(derivation);
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(updateDto.DoctorId)).ReturnsAsync(doctor);
            _mockDepartmentHeadRepo.Setup(r => r.GetByIdAsync(updateDto.DepartmentHeadId)).ReturnsAsync(departmentHead);

            // ACT
            var result = await _service.UpdateAsync(consultationId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("El doctor tratante debe pertenecer al mismo departamento destino", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<ConsultationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var consultationId = Guid.NewGuid();
            var departmentToId = Guid.NewGuid();
            var derivationId = Guid.NewGuid();

            var existingConsultation = new ConsultationDerivation(
                consultationId,
                "Diagnosis",
                derivationId,
                DateTime.Now,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            var updateDto = new UpdateConsultationDerivationDto
            {
                Diagnosis = "New Diagnosis",
                DoctorId = Guid.NewGuid(),
                DepartmentHeadId = Guid.NewGuid()
            };

            var derivation = new Derivation(
                derivationId,
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                departmentToId
            );

            var doctor = new Doctor(
                updateDto.DoctorId,
                "12345678",
                "Dr. Test",
                "Active",
                departmentToId
            );

            var departmentHead = new DepartmentHead(
                updateDto.DepartmentHeadId,
                Guid.NewGuid(),
                departmentToId,
                DateTime.Now
            );

            _mockRepository.Setup(r => r.GetByIdAsync(consultationId)).ReturnsAsync(existingConsultation);
            _mockDerivationRepo.Setup(r => r.GetByIdAsync(derivationId)).ReturnsAsync(derivation);
            _mockDoctorRepo.Setup(r => r.GetByIdAsync(updateDto.DoctorId)).ReturnsAsync(doctor);
            _mockDepartmentHeadRepo.Setup(r => r.GetByIdAsync(updateDto.DepartmentHeadId)).ReturnsAsync(departmentHead);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<ConsultationDerivation>())).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.UpdateAsync(consultationId, updateDto);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al actualizar la consulta", result.ErrorMessage);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ValidId_ReturnsSuccessResult()
        {
            // ARRANGE
            var consultationId = Guid.NewGuid();
            var consultation = new ConsultationDerivation(
                consultationId,
                "Diagnosis",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            _mockRepository.Setup(r => r.GetByIdAsync(consultationId)).ReturnsAsync(consultation);
            _mockRepository.Setup(r => r.DeleteAsync(consultation)).Returns(Task.CompletedTask);

            // ACT
            var result = await _service.DeleteAsync(consultationId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.GetByIdAsync(consultationId), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(consultation), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ConsultationNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var consultationId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(consultationId)).ReturnsAsync((ConsultationDerivation?)null);

            // ACT
            var result = await _service.DeleteAsync(consultationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Record not found", result.ErrorMessage);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<ConsultationDerivation>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var consultationId = Guid.NewGuid();
            var consultation = new ConsultationDerivation(
                consultationId,
                "Diagnosis",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            _mockRepository.Setup(r => r.GetByIdAsync(consultationId)).ReturnsAsync(consultation);
            _mockRepository.Setup(r => r.DeleteAsync(consultation)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.DeleteAsync(consultationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al eliminar la consulta", result.ErrorMessage);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsSuccessWithConsultation()
        {
            // ARRANGE
            var consultationId = Guid.NewGuid();
            var consultation = new ConsultationDerivation(
                consultationId,
                "Diagnosis",
                Guid.NewGuid(),
                DateTime.Now,
                Guid.NewGuid(),
                Guid.NewGuid()
            );

            var consultationDto = new ConsultationDerivationDto
            {
                ConsultationDerivationId = consultationId,
                Diagnosis = "Diagnosis"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(consultationId)).ReturnsAsync(consultation);
            _mockMapper.Setup(m => m.Map<ConsultationDerivationDto>(consultation)).Returns(consultationDto);

            // ACT
            var result = await _service.GetByIdAsync(consultationId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(consultationId, result.Value.ConsultationDerivationId);
            _mockRepository.Verify(r => r.GetByIdAsync(consultationId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ConsultationNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var consultationId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(consultationId)).ReturnsAsync((ConsultationDerivation?)null);

            // ACT
            var result = await _service.GetByIdAsync(consultationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Record not found", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var consultationId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(consultationId)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.GetByIdAsync(consultationId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener consulta", result.ErrorMessage);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_ConsultationsExist_ReturnsSuccessWithList()
        {
            // ARRANGE
            var consultations = new List<ConsultationDerivation>
            {
                new ConsultationDerivation(Guid.NewGuid(), "Diagnosis1", Guid.NewGuid(), DateTime.Now, Guid.NewGuid(), Guid.NewGuid()),
                new ConsultationDerivation(Guid.NewGuid(), "Diagnosis2", Guid.NewGuid(), DateTime.Now, Guid.NewGuid(), Guid.NewGuid())
            };

            var consultationDtos = new List<ConsultationDerivationDto>
            {
                new ConsultationDerivationDto { ConsultationDerivationId = consultations[0].ConsultationDerivationId },
                new ConsultationDerivationDto { ConsultationDerivationId = consultations[1].ConsultationDerivationId }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(consultations);
            _mockMapper.Setup(m => m.Map<IEnumerable<ConsultationDerivationDto>>(consultations)).Returns(consultationDtos);

            // ACT
            var result = await _service.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_NoConsultations_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var emptyList = new List<ConsultationDerivation>();
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<IEnumerable<ConsultationDerivationDto>>(emptyList)).Returns(new List<ConsultationDerivationDto>());

            // ACT
            var result = await _service.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            _mockRepository.Setup(r => r.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.GetAllAsync();

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener consultas", result.ErrorMessage);
        }

        #endregion

        #region GetByDateRangeAsync Tests

        [Fact]
        public async Task GetByDateRangeAsync_ConsultationsFound_ReturnsSuccessWithList()
        {
            // ARRANGE
            var patientId = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;

            var consultations = new List<ConsultationDerivation>
            {
                new ConsultationDerivation(Guid.NewGuid(), "Diagnosis1", Guid.NewGuid(), DateTime.Now.AddDays(-15), Guid.NewGuid(), Guid.NewGuid())
            };

            var consultationDtos = new List<ConsultationDerivationDto>
            {
                new ConsultationDerivationDto { PatientId = patientId }
            };

            _mockRepository.Setup(r => r.GetByDateRangeAsync(patientId, startDate, endDate)).ReturnsAsync(consultations);
            _mockMapper.Setup(m => m.Map<IEnumerable<ConsultationDerivationDto>>(consultations)).Returns(consultationDtos);

            // ACT
            var result = await _service.GetByDateRangeAsync(patientId, startDate, endDate);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task GetByDateRangeAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var patientId = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;

            _mockRepository.Setup(r => r.GetByDateRangeAsync(patientId, startDate, endDate)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.GetByDateRangeAsync(patientId, startDate, endDate);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener consultas", result.ErrorMessage);
        }

        #endregion

        #region GetLast10ByPatientIdAsync Tests

        [Fact]
        public async Task GetLast10ByPatientIdAsync_ConsultationsFound_ReturnsSuccessWithList()
        {
            // ARRANGE
            var patientId = Guid.NewGuid();
            var consultations = new List<ConsultationDerivation>
            {
                new ConsultationDerivation(Guid.NewGuid(), "Diagnosis1", Guid.NewGuid(), DateTime.Now, Guid.NewGuid(), Guid.NewGuid()),
                new ConsultationDerivation(Guid.NewGuid(), "Diagnosis2", Guid.NewGuid(), DateTime.Now.AddDays(-1), Guid.NewGuid(), Guid.NewGuid())
            };

            var consultationDtos = new List<ConsultationDerivationDto>
            {
                new ConsultationDerivationDto { PatientId = patientId },
                new ConsultationDerivationDto { PatientId = patientId }
            };

            _mockRepository.Setup(r => r.GetLast10ByPatientIdAsync(patientId)).ReturnsAsync(consultations);
            _mockMapper.Setup(m => m.Map<IEnumerable<ConsultationDerivationDto>>(consultations)).Returns(consultationDtos);

            // ACT
            var result = await _service.GetLast10ByPatientIdAsync(patientId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
        }

        [Fact]
        public async Task GetLast10ByPatientIdAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var patientId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetLast10ByPatientIdAsync(patientId)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _service.GetLast10ByPatientIdAsync(patientId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al obtener consultas", result.ErrorMessage);
        }

        #endregion
    } 
}       