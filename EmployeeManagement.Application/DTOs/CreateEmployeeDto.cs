using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Application.DTOs;

public class CreateEmployeeDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string DocumentNumber { get; set; } = string.Empty;

    [Required]
    [MinLength(1)]
    public List<string> PhoneNumbers { get; set; } = new();

    public int? ManagerId { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public int Role { get; set; }
}
