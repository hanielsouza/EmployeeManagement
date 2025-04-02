using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Domain.Entities;

public class EmployeePhone
{
    public int Id { get; set; }

    [Required]
    [Phone]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
}
