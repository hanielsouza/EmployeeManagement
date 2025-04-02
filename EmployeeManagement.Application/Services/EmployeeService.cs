using EmployeeManagement.Application.DTOs;
using EmployeeManagement.Application.Interfaces;
using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Interfaces;
using EmployeeManagement.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Application.Services;

public class EmployeeService(IEmployeeRepository employeeRepository, ILogger<EmployeeService> logger) : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository;
    private readonly ILogger<EmployeeService> _logger = logger;

    public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();
        return employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email,
            DocumentNumber = e.DocumentNumber,
            DateOfBirth = e.DateOfBirth,
            Role = e.Role,
            ManagerId = e.ManagerId,
            PhoneNumbers = e.PhoneNumbers.Select(p => p.PhoneNumber).ToList()
        });
    }

    public async Task<EmployeeDto?> GetEmployeeAsync(int id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null) return null;

        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            DocumentNumber = employee.DocumentNumber,
            DateOfBirth = employee.DateOfBirth,
            Role = employee.Role,
            ManagerId = employee.ManagerId,
            PhoneNumbers = employee.PhoneNumbers.Select(p => p.PhoneNumber).ToList()
        };
    }

    public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeDto createEmployeeDto, UserRole creatorRole)
    {
        _logger.LogInformation("Creating new employee with email: {Email}, Role: {Role}",
       createEmployeeDto.Email, createEmployeeDto.Role);

        // Validação de idade
        var age = DateTime.Today.Year - createEmployeeDto.DateOfBirth.Year;
        if (createEmployeeDto.DateOfBirth.Date > DateTime.Today.AddYears(-age))
            age--;

        if (age < 18)
        {
            _logger.LogWarning("Attempt to create underage employee. Birth Date: {BirthDate}", createEmployeeDto.DateOfBirth);
            throw new InvalidOperationException("Employee must be at least 18 years old");
        }

        // Verifica se o criador está tentando criar alguém com role superior
        if ((UserRole)createEmployeeDto.Role > creatorRole)
        {
            _logger.LogWarning("Attempt to create employee with higher permissions. Creator Role: {CreatorRole}, Attempted Role: {AttemptedRole}",
            creatorRole, createEmployeeDto.Role);
            throw new InvalidOperationException("Cannot create an employee with higher permissions than your own");
        }

        var employee = new Employee
        {
            FirstName = createEmployeeDto.FirstName,
            LastName = createEmployeeDto.LastName,
            Email = createEmployeeDto.Email,
            DocumentNumber = createEmployeeDto.DocumentNumber,
            DateOfBirth = createEmployeeDto.DateOfBirth,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(createEmployeeDto.Password),
            Role = (UserRole)createEmployeeDto.Role,
            ManagerId = createEmployeeDto.ManagerId,
            PhoneNumbers = createEmployeeDto.PhoneNumbers.Select(p => new EmployeePhone { PhoneNumber = p }).ToList()
        };

        await _employeeRepository.AddAsync(employee);
        await _employeeRepository.SaveChangesAsync();

        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            DocumentNumber = employee.DocumentNumber,
            DateOfBirth = employee.DateOfBirth,
            Role = employee.Role,
            ManagerId = employee.ManagerId,
            PhoneNumbers = employee.PhoneNumbers.Select(p => p.PhoneNumber).ToList()
        };
    }

    public async Task<EmployeeDto> UpdateEmployeeAsync(int id, UpdateEmployeeDto updateEmployeeDto, UserRole updaterRole)
    {
        _logger.LogInformation("Updating employee with ID: {Id}", id);
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            _logger.LogWarning("Employee not found with ID: {Id}", id);
            throw new KeyNotFoundException($"Employee with ID {id} not found.");
        }

        if (employee.Role > updaterRole)
        {
            _logger.LogWarning("Attempt to update employee with higher role. Updater Role: {UpdaterRole}, Target Employee Role: {EmployeeRole}",
                updaterRole, employee.Role);
            throw new InvalidOperationException("Cannot update an employee with higher permissions than your own");
        }

        employee.FirstName = updateEmployeeDto.FirstName;
        employee.LastName = updateEmployeeDto.LastName;
        employee.Email = updateEmployeeDto.Email;

        if (!string.IsNullOrEmpty(updateEmployeeDto.Password))
        {
            employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateEmployeeDto.Password);
        }

        // Update phone numbers
        employee.PhoneNumbers.Clear();
        employee.PhoneNumbers = updateEmployeeDto.PhoneNumbers.Select(p => new EmployeePhone { PhoneNumber = p }).ToList();

        await _employeeRepository.UpdateAsync(employee);
        await _employeeRepository.SaveChangesAsync();

        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            DocumentNumber = employee.DocumentNumber,
            DateOfBirth = employee.DateOfBirth,
            Role = employee.Role,
            ManagerId = employee.ManagerId,
            PhoneNumbers = employee.PhoneNumbers.Select(p => p.PhoneNumber).ToList()
        };
    }

    public async Task DeleteEmployeeAsync(int id, UserRole deleterRole)
    {
        _logger.LogInformation("Deleting employee with ID: {Id}", id);
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee == null)
        {
            _logger.LogWarning("Attempt to delete non-existent employee with ID: {Id}", id);
            throw new KeyNotFoundException($"Employee with ID {id} not found.");
        }

        if (employee.Role > deleterRole)
        {
            _logger.LogWarning("Attempt to delete employee with higher role. Deleter Role: {DeleterRole}, Target Employee Role: {EmployeeRole}",
                deleterRole, employee.Role);
            throw new InvalidOperationException("Cannot delete an employee with higher permissions than your own");
        }

        await _employeeRepository.DeleteAsync(id);
        await _employeeRepository.SaveChangesAsync();
    }
}