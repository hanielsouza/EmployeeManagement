using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Domain.Enums;

namespace EmployeeManagement.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync();
    Task<EmployeeDto?> GetEmployeeAsync(int id);
    Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto, UserRole creatorRole);
    Task<EmployeeDto> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateEmployeeDto, UserRole updaterRole);
    Task DeleteEmployeeAsync(int id, UserRole deleterRole);
}
