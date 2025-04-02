using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace EmployeeManagement.Tests;

public class EmployeeServiceTests
{
    private readonly Mock<IEmployeeRepository> _mockRepo;
    private readonly Mock<ILogger<EmployeeService>> _mockLogger;
    private readonly EmployeeService _service;

    public EmployeeServiceTests()
    {
        _mockRepo = new Mock<IEmployeeRepository>();
        _mockLogger = new Mock<ILogger<EmployeeService>>();
        _service = new EmployeeService(_mockRepo.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllEmployees_ShouldReturnAllEmployees()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe", PhoneNumbers = [] },
            new() { Id = 2, FirstName = "Jane", LastName = "Smith", PhoneNumbers = [] }
        };

        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(employees);

        // Act
        var result = await _service.GetAllEmployeesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().FirstName.Should().Be("John");
    }

    [Fact]
    public async Task GetEmployee_WhenExists_ShouldReturnEmployee()
    {
        // Arrange
        var employee = new Employee
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            PhoneNumbers = []
        };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);

        // Act
        var result = await _service.GetEmployeeAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("John");
    }

    [Fact]
    public async Task GetEmployee_WhenNotExists_ShouldReturnNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.GetEmployeeAsync(1);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(UserRole.Employee)]
    [InlineData(UserRole.Leader)]
    public async Task CreateEmployee_AsDirector_ShouldSucceed(UserRole newEmployeeRole)
    {
        // Arrange
        var dto = new CreateEmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            DocumentNumber = "12345678",
            DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Password = "Password123!",
            Role = (int)newEmployeeRole,
            PhoneNumbers = ["1234567890"]
        };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateEmployeeAsync(dto, UserRole.Director);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("John");
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once);
    }

    [Fact]
    public async Task CreateEmployee_AsLeader_ShouldSucceed()
    {
        // Arrange
        var dto = new CreateEmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            DocumentNumber = "12345678",
            DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Password = "Password123!",
            Role = (int)UserRole.Employee,
            PhoneNumbers = ["1234567890"]
        };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateEmployeeAsync(dto, UserRole.Leader);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("John");
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once);
    }


    [Fact]
    public async Task CreateEmployee_AsEmployee_ShouldSucceed()
    {
        // Arrange
        var dto = new CreateEmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            DocumentNumber = "12345678",
            DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Password = "Password123!",
            Role = (int)UserRole.Employee,
            PhoneNumbers = ["1234567890"]
        };

        _mockRepo.Setup(r => r.AddAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateEmployeeAsync(dto, UserRole.Employee);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("John");
        _mockRepo.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once);
    }

    [Theory]
    [InlineData(UserRole.Employee, UserRole.Leader, "Cannot create an employee with higher permissions than your own")]
    [InlineData(UserRole.Employee, UserRole.Director, "Cannot create an employee with higher permissions than your own")]
    [InlineData(UserRole.Leader, UserRole.Director, "Cannot create an employee with higher permissions than your own")]
    public async Task CreateEmployee_WithInsufficientPermissions_ShouldThrowException(UserRole creatorRole, UserRole newEmployeeRole, string expectedError)
    {
        // Arrange
        var dto = new CreateEmployeeDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            DocumentNumber = "12345678",
            DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Password = "Password123!",
            Role = (int)newEmployeeRole,
            PhoneNumbers = ["1234567890"]
        };

        // Act & Assert
        await _service.Invoking(s => s.CreateEmployeeAsync(dto, creatorRole))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage(expectedError);
    }

    [Fact]
    public async Task UpdateEmployee_WhenExists_ShouldSucceed()
    {
        // Arrange
        var employee = new Employee
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            PhoneNumbers = [],
            Role = UserRole.Employee
        };

        var updateDto = new UpdateEmployeeDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            Password = "NewPassword123!",
            PhoneNumbers = ["9876543210"],
        };

        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
        _mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Employee>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateEmployeeAsync(1, updateDto, UserRole.Director);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Smith");
    }

    [Fact]
    public async Task UpdateEmployee_WhenNotExists_ShouldThrowException()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Employee?)null);

        var updateDto = new UpdateEmployeeDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com",
            PhoneNumbers = []
        };

        // Act & Assert
        await _service.Invoking(s => s.UpdateEmployeeAsync(1, updateDto, UserRole.Director))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Employee with ID 1 not found.");
    }

    [Fact]
    public async Task DeleteEmployee_WhenExists_ShouldSucceed()
    {
        // Arrange
        var employee = new Employee { Id = 1, Role = UserRole.Employee };
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
        _mockRepo.Setup(r => r.DeleteAsync(1)).Returns(Task.CompletedTask);

        // Act & Assert
        await _service.Invoking(s => s.DeleteEmployeeAsync(1, UserRole.Director))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task DeleteEmployee_WhenNotExists_ShouldThrowException()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Employee?)null);

        // Act & Assert
        await _service.Invoking(s => s.DeleteEmployeeAsync(1, UserRole.Director))
            .Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("Employee with ID 1 not found.");
    }
}