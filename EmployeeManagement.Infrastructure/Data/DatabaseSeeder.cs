using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedDataAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EmployeeDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<EmployeeDbContext>>();

        try
        {
            await context.Database.MigrateAsync();

            if (!await context.Employees.AnyAsync())
            {
                // Criar diretor
                var director = new Employee
                {
                    FirstName = "João",
                    LastName = "Silva",
                    Email = "diretor@empresa.com",
                    DocumentNumber = "12345678900",
                    DateOfBirth = new DateTime(1980, 1, 1),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Role = UserRole.Director,
                    PhoneNumbers = new List<EmployeePhone>
                    {
                        new() { PhoneNumber = "11999999999" }
                    }
                };
                context.Employees.Add(director);
                await context.SaveChangesAsync();

                // Criar líder
                var leader = new Employee
                {
                    FirstName = "Maria",
                    LastName = "Santos",
                    Email = "lider@empresa.com",
                    DocumentNumber = "98765432100",
                    DateOfBirth = new DateTime(1985, 5, 15),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                    Role = UserRole.Leader,
                    ManagerId = director.Id,
                    PhoneNumbers = new List<EmployeePhone>
                    {
                        new() { PhoneNumber = "11988888888" }
                    }
                };
                context.Employees.Add(leader);
                await context.SaveChangesAsync();

                // Criar funcionários
                var employees = new[]
                {
                    new Employee
                    {
                        FirstName = "Pedro",
                        LastName = "Oliveira",
                        Email = "pedro@empresa.com",
                        DocumentNumber = "11122233344",
                        DateOfBirth = new DateTime(1990, 3, 20),
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                        Role = UserRole.Employee,
                        ManagerId = leader.Id,
                        PhoneNumbers = new List<EmployeePhone>
                        {
                            new() { PhoneNumber = "11977777777" }
                        }
                    },
                    new Employee
                    {
                        FirstName = "Ana",
                        LastName = "Costa",
                        Email = "ana@empresa.com",
                        DocumentNumber = "44433322211",
                        DateOfBirth = new DateTime(1992, 7, 10),
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                        Role = UserRole.Employee,
                        ManagerId = leader.Id,
                        PhoneNumbers = new List<EmployeePhone>
                        {
                            new() { PhoneNumber = "11966666666" }
                        }
                    }
                };

                context.Employees.AddRange(employees);
                await context.SaveChangesAsync();

                logger.LogInformation("Dados de teste foram adicionados com sucesso.");
            }
            else
            {
                logger.LogInformation("Banco de dados já contém dados. O seed não foi necessário.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ocorreu um erro durante o seed do banco de dados.");
            throw;
        }
    }
}
