using FluentAssertions;
using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Model;
using MidtermTeno.AttendanceManagementSysttem.Service;
using Moq;

namespace AMS_DBTC_API.Tests.Services
{
    public class DepartmentServiceTests
    {
        private readonly Mock<IDepartmentRepository> _repo = new();
        private readonly DepartmentService _sut;

        public DepartmentServiceTests() => _sut = new DepartmentService(_repo.Object);

        [Fact]
        public async Task CreateAsync_ShouldRejectEmptyCode()
        {
            var result = await _sut.CreateAsync(new DepartmentDTO { DepartmentName = "IT" });
            result.ErrorMessage.Should().Be("DepartmentCode is required.");
        }

        [Fact]
        public async Task CreateAsync_ShouldSucceed_WhenValid()
        {
            _repo.Setup(r => r.AddAsync(It.IsAny<Department>()))
                .ReturnsAsync((Department d) =>
                {
                    d.DepartmentId = 1;
                    return d;
                });

            var result = await _sut.CreateAsync(new DepartmentDTO
            {
                DepartmentCode = "CIT",
                DepartmentName = "College of IT"
            });

            result.IsSuccess.Should().BeTrue();
            result.Data!.DepartmentId.Should().Be(1);
        }
    }
}
