using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.DTOs;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public List<string> PhoneNumbers { get; set; } = new();
    public int? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public UserRole Role { get; set; }
}
