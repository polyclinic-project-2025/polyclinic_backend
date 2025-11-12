using Xunit;
using Moq;
using AutoMapper;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using FluentAssertions;
using PolyclinicDomain.IRepositories;
using PolyclinicApplication.Services.Implementations;
using PolyclinicApplication.DTOs.Departments;
using PolyclinicDomain.Entities;

namespace PolyclinicTests.UnitTests
{
    public class DepartmentServiceTests
    {
        private readonly Mock<IDepartmentRepository> _repositoryMock;
        private readonly IMapper _mapper;
        private readonly DepartmentService _service;

        public DepartmentServiceTests()
        {
            _repositoryMock = new Mock<IDepartmentRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Department, DepartmentDto>();
                cfg.CreateMap<CreateDepartmentDto, Department>()
                   .ConstructUsing(dto => new Department(Guid.NewGuid(), dto.Name, dto.HeadId));
            });

            _mapper = config.CreateMapper();
            _service = new DepartmentService(_repositoryMock.Object, _mapper);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateDepartment_WhenNameIsUnique()
        {
            // Arrange
            var dto = new CreateDepartmentDto { Name = "Cardiology" };
            _repositoryMock.Setup(r => r.ExistsByNameAsync("Cardiology")).ReturnsAsync(false);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Cardiology");
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Department>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenDuplicateName()
        {
            var dto = new CreateDepartmentDto { Name = "Pediatrics" };
            _repositoryMock.Setup(r => r.ExistsByNameAsync("Pediatrics")).ReturnsAsync(true);

            Func<Task> act = async () => await _service.CreateAsync(dto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("A department with the same name already exists.");
        }
    }
}
