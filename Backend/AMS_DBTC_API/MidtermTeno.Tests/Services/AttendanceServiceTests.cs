using FluentAssertions;
using AMS_DBTC_API.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Model;
using MidtermTeno.AttendanceManagementSysttem.Service;
using Moq;

namespace AMS_DBTC_API.Tests.Services
{
    public class AttendanceServiceTests
    {
        private readonly Mock<IAttendanceRepository> _attendanceRepo = new();
        private readonly Mock<IEnrollmentRepository> _enrollmentRepo = new();
        private readonly Mock<ICourseRepository> _courseRepo = new();
        private readonly AttendanceService _sut;

        public AttendanceServiceTests()
        {
            _sut = new AttendanceService(_attendanceRepo.Object, _enrollmentRepo.Object, _courseRepo.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldReject_WhenStudentNotEnrolled()
        {
            _enrollmentRepo.Setup(r => r.IsActiveEnrollmentAsync(1, 2)).ReturnsAsync(false);

            var result = await _sut.CreateAsync(new AttendanceRecordDTO
            {
                StudentId = 1,
                CourseId = 2,
                AttendanceDate = DateTime.UtcNow.Date,
                Status = AttendanceStatus.Present
            });

            result.ErrorMessage.Should().Be("Student is not actively enrolled in this course.");
        }

        [Fact]
        public async Task MarkAsync_ShouldReject_WhenTeacherDoesNotOwnCourse()
        {
            _enrollmentRepo.Setup(r => r.IsActiveEnrollmentAsync(1, 2)).ReturnsAsync(true);
            _courseRepo.Setup(r => r.IsOwnedByTeacherAsync(2, 99)).ReturnsAsync(false);

            var result = await _sut.MarkAsync(new MarkAttendanceDTO
            {
                StudentId = 1,
                CourseId = 2,
                AttendanceDate = DateTime.UtcNow.Date,
                Status = AttendanceStatus.Present
            }, teacherId: 99);

            result.ErrorMessage.Should().Be("You are not assigned to this course.");
        }
    }
}
