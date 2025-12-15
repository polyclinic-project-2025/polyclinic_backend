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
    public class NurseServiceTests
    {
        private readonly Mock<INurseRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly NurseService _nurseService;

        public NurseServiceTests()
        {
            _mockRepository = new Mock<INurseRepository>();
            _mockMapper = new Mock<IMapper>();
            _nurseService = new NurseService(_mockRepository.Object, _mockMapper.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ValidNurse_ReturnsSuccessResult()
        {
            // ARRANGE
            var createRequest = new CreateNurseRequest
            {
                Identification = "12345678",
                Name = "Enfermera Ana López",
                EmploymentStatus = "Active"
            };

            var nurse = new Nurse(
                Guid.NewGuid(),
                createRequest.Identification,
                createRequest.Name,
                createRequest.EmploymentStatus
            );

            var nurseResponse = new NurseResponse
            {
                EmployeeId = nurse.EmployeeId,
                Name = nurse.Name,
                Identification = nurse.Identification
            };

            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(createRequest.Identification)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Nurse>())).ReturnsAsync(nurse);
            _mockMapper.Setup(m => m.Map<NurseResponse>(It.IsAny<Nurse>())).Returns(nurseResponse);

            // ACT
            var result = await _nurseService.CreateAsync(createRequest);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(createRequest.Name, result.Value.Name);
            Assert.Equal(createRequest.Identification, result.Value.Identification);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Nurse>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DuplicateIdentification_ReturnsFailureResult()
        {
            // ARRANGE
            var createRequest = new CreateNurseRequest
            {
                Identification = "12345678",
                Name = "Test Nurse",
                EmploymentStatus = "Active"
            };

            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(createRequest.Identification)).ReturnsAsync(true);

            // ACT
            var result = await _nurseService.CreateAsync(createRequest);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Ya existe un empleado con esta identificación", result.ErrorMessage);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Nurse>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var createRequest = new CreateNurseRequest
            {
                Identification = "12345678",
                Name = "Test Nurse",
                EmploymentStatus = "Active"
            };

            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(createRequest.Identification)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Nurse>())).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _nurseService.CreateAsync(createRequest);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al guardar el enfermero", result.ErrorMessage);
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_NursesExist_ReturnsSuccessWithList()
        {
            // ARRANGE
            var nurses = new List<Nurse>
            {
                new Nurse(Guid.NewGuid(), "12345678", "Enfermera Ana", "Active"),
                new Nurse(Guid.NewGuid(), "87654321", "Enfermero Carlos", "Active")
            };

            var nurseResponses = new List<NurseResponse>
            {
                new NurseResponse { EmployeeId = nurses[0].EmployeeId, Name = "Enfermera Ana" },
                new NurseResponse { EmployeeId = nurses[1].EmployeeId, Name = "Enfermero Carlos" }
            };

            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(nurses);
            _mockMapper.Setup(m => m.Map<IEnumerable<NurseResponse>>(nurses)).Returns(nurseResponses);

            // ACT
            var result = await _nurseService.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count());
            _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_NoNurses_ReturnsSuccessWithEmptyList()
        {
            // ARRANGE
            var emptyList = new List<Nurse>();
            _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(emptyList);
            _mockMapper.Setup(m => m.Map<IEnumerable<NurseResponse>>(emptyList)).Returns(new List<NurseResponse>());

            // ACT
            var result = await _nurseService.GetAllAsync();

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsSuccessWithNurse()
        {
            // ARRANGE
            var nurseId = Guid.NewGuid();
            var nurse = new Nurse(nurseId, "12345678", "Enfermera Ana", "Active");
            var nurseResponse = new NurseResponse { EmployeeId = nurseId, Name = "Enfermera Ana" };

            _mockRepository.Setup(r => r.GetByIdAsync(nurseId)).ReturnsAsync(nurse);
            _mockMapper.Setup(m => m.Map<NurseResponse>(nurse)).Returns(nurseResponse);

            // ACT
            var result = await _nurseService.GetByIdAsync(nurseId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(nurseId, result.Value.EmployeeId);
            Assert.Equal("Enfermera Ana", result.Value.Name);
            _mockRepository.Verify(r => r.GetByIdAsync(nurseId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_NurseNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var nurseId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(nurseId)).ReturnsAsync((Nurse?)null);

            // ACT
            var result = await _nurseService.GetByIdAsync(nurseId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Empleado no encontrado", result.ErrorMessage);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_ValidUpdate_ReturnsSuccessResult()
        {
            // ARRANGE
            var nurseId = Guid.NewGuid();
            var existingNurse = new Nurse(nurseId, "12345678", "Old Name", "Active");
            var updateRequest = new UpdateNurseRequest
            {
                Name = "New Name",
                Identification = "87654321",
                EmploymentStatus = "Inactive"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(nurseId)).ReturnsAsync(existingNurse);
            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(updateRequest.Identification)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Nurse>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _nurseService.UpdateAsync(nurseId, updateRequest);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Nurse>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NurseNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var nurseId = Guid.NewGuid();
            var updateRequest = new UpdateNurseRequest { Name = "New Name" };
            _mockRepository.Setup(r => r.GetByIdAsync(nurseId)).ReturnsAsync((Nurse?)null);

            // ACT
            var result = await _nurseService.UpdateAsync(nurseId, updateRequest);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Enfermero no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Nurse>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_DuplicateIdentification_ReturnsFailureResult()
        {
            // ARRANGE
            var nurseId = Guid.NewGuid();
            var existingNurse = new Nurse(nurseId, "12345678", "Nurse Name", "Active");
            var updateRequest = new UpdateNurseRequest { Identification = "87654321" };

            _mockRepository.Setup(r => r.GetByIdAsync(nurseId)).ReturnsAsync(existingNurse);
            _mockRepository.Setup(r => r.ExistsByIdentificationAsync(updateRequest.Identification)).ReturnsAsync(true);

            // ACT
            var result = await _nurseService.UpdateAsync(nurseId, updateRequest);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Ya existe un enfermero con esta identificación", result.ErrorMessage);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Nurse>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_SameIdentification_SkipsValidation()
        {
            // ARRANGE
            var nurseId = Guid.NewGuid();
            var existingNurse = new Nurse(nurseId, "12345678", "Nurse Name", "Active");
            var updateRequest = new UpdateNurseRequest
            {
                Identification = "12345678",
                Name = "Updated Name"
            };

            _mockRepository.Setup(r => r.GetByIdAsync(nurseId)).ReturnsAsync(existingNurse);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Nurse>())).Returns(Task.CompletedTask);

            // ACT
            var result = await _nurseService.UpdateAsync(nurseId, updateRequest);

            // ASSERT
            Assert.True(result.IsSuccess);
            _mockRepository.Verify(r => r.ExistsByIdentificationAsync(It.IsAny<string>()), Times.Never);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Nurse>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var nurseId = Guid.NewGuid();
            var existingNurse = new Nurse(nurseId, "12345678", "Nurse Name", "Active");
            var updateRequest = new UpdateNurseRequest { Name = "New Name" };

            _mockRepository.Setup(r => r.GetByIdAsync(nurseId)).ReturnsAsync(existingNurse);
            _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Nurse>())).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _nurseService.UpdateAsync(nurseId, updateRequest);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al actualizar el enfermero", result.ErrorMessage);
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_ValidId_ReturnsSuccessResult()
        {
            // ARRANGE
            var nurseId = Guid.NewGuid();
            var nurse = new Nurse(nurseId, "12345678", "Enfermera Ana", "Active");

            _mockRepository.Setup(r => r.GetByIdAsync(nurseId)).ReturnsAsync(nurse);
            _mockRepository.Setup(r => r.DeleteAsync(nurse)).Returns(Task.CompletedTask);

            // ACT
            var result = await _nurseService.DeleteAsync(nurseId);

            // ASSERT
            Assert.True(result.IsSuccess);
            Assert.True(result.Value);
            _mockRepository.Verify(r => r.GetByIdAsync(nurseId), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(nurse), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_NurseNotFound_ReturnsFailureResult()
        {
            // ARRANGE
            var nurseId = Guid.NewGuid();
            _mockRepository.Setup(r => r.GetByIdAsync(nurseId)).ReturnsAsync((Nurse?)null);

            // ACT
            var result = await _nurseService.DeleteAsync(nurseId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Empleado no encontrado", result.ErrorMessage);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Nurse>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_RepositoryThrowsException_ReturnsFailureResult()
        {
            // ARRANGE
            var nurseId = Guid.NewGuid();
            var nurse = new Nurse(nurseId, "12345678", "Enfermera Ana", "Active");

            _mockRepository.Setup(r => r.GetByIdAsync(nurseId)).ReturnsAsync(nurse);
            _mockRepository.Setup(r => r.DeleteAsync(nurse)).ThrowsAsync(new Exception("Database error"));

            // ACT
            var result = await _nurseService.DeleteAsync(nurseId);

            // ASSERT
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al eliminar el empleado", result.ErrorMessage);
        }

        #endregion
    }
}