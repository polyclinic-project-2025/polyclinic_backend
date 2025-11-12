using Xunit;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using PolyclinicAPI;
using PolyclinicApplication.DTOs.Departments;

namespace PolyclinicTests.IntegrationTests
{
    public class DepartmentControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public DepartmentControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_ShouldReturn201_WhenValid()
        {
            var dto = new CreateDepartmentDto { Name = "Test Department" };

            var response = await _client.PostAsJsonAsync("/api/departments", dto);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }
    }
}
