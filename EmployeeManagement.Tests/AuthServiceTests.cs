using EmployeeManagement.Application.Services;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using EmployeeManagement.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace EmployeeManagement.Tests;

public class AuthServiceTests
{
    private readonly Mock<IEmployeeRepository> _mockRepo;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _mockRepo = new Mock<IEmployeeRepository>();
        _mockConfig = new Mock<IConfiguration>();

        // Setup JWT configuration
        _mockConfig.Setup(c => c["Jwt:Key"]).Returns("your-256-bit-secret-key-here-for-testing-purposes-only");
        _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("test-issuer");
        _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("test-audience");

        _service = new AuthService(_mockRepo.Object, _mockConfig.Object);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var email = "john@example.com";
        var password = "Password123!";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var employee = new Employee
        {
            Id = 1,
            Email = email,
            PasswordHash = hashedPassword,
            Role = UserRole.Employee
        };

        _mockRepo.Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync(employee);

        // Act
        var token = await _service.AuthenticateAsync(email, password);

        // Assert
        token.Should().NotBeNull();

        // Validate token structure
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Claims.Should().Contain(c => c.Type == "email" && c.Value == email);
        jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == UserRole.Employee.ToString());
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidEmail_ShouldReturnNull()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync((Employee?)null);

        // Act
        var token = await _service.AuthenticateAsync("invalid@example.com", "anypassword");

        // Assert
        token.Should().BeNull();
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var email = "john@example.com";
        var correctPassword = "Password123!";
        var wrongPassword = "WrongPassword123!";

        var employee = new Employee
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(correctPassword)
        };

        _mockRepo.Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync(employee);

        // Act
        var token = await _service.AuthenticateAsync(email, wrongPassword);

        // Assert
        token.Should().BeNull();
    }

    [Fact]
    public async Task AuthenticateAsync_WithMissingJwtKey_ShouldThrowException()
    {
        // Arrange
        _mockConfig.Setup(c => c["Jwt:Key"]).Returns((string?)null);

        var email = "john@example.com";
        var password = "Password123!";
        var employee = new Employee
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
        };

        _mockRepo.Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync(employee);

        // Act & Assert
        await _service.Invoking(s => s.AuthenticateAsync(email, password))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("JWT Key not configured");
    }
}