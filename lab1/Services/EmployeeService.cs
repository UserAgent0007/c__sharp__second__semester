using lab1.Models;
using lab1.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace lab1.Services;

public class EmployeeService(IDbContextFactory<ProjectContractsContext> factory)
{
    public async Task<List<Employee>> GetAllAsync()
    {
        await using var db = factory.CreateDbContext();
        return await db.Employees.AsNoTracking().Include(e => e.Position).ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        await using var db = factory.CreateDbContext();
        return await db.Employees.AsNoTracking()
            .Include(e => e.Position)
            .Include(e => e.EmployeeProjects).ThenInclude(ep => ep.Project)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<string?> CreateAsync(EmployeeInput input)
    {
        await using var db = factory.CreateDbContext();

        if (input.PositionId is not int positionId || !await db.Positions.AnyAsync(p => p.Id == positionId))
            return "Position does not exist.";

        db.Employees.Add(new Employee
        {
            FirstName = input.FirstName,
            LastName = input.LastName,
            MiddleName = input.MiddleName,
            PositionId = positionId,
            Address = input.Address,
            Characteristic = input.Characteristic
        });
        await db.SaveChangesAsync();
        return null;
    }

    public async Task<string?> UpdateAsync(int id, EmployeeInput input)
    {
        await using var db = factory.CreateDbContext();

        var employee = await db.Employees.FindAsync(id);
        if (employee == null) return "Employee not found.";

        if (input.PositionId is not int positionId || !await db.Positions.AnyAsync(p => p.Id == positionId))
            return "Position does not exist.";

        employee.FirstName = input.FirstName;
        employee.LastName = input.LastName;
        employee.MiddleName = input.MiddleName;
        employee.PositionId = positionId;
        employee.Address = input.Address;
        employee.Characteristic = input.Characteristic;
        await db.SaveChangesAsync();
        return null;
    }

    public async Task<string?> DeleteAsync(int id)
    {
        await using var db = factory.CreateDbContext();
        var employee = await db.Employees.Include(e => e.EmployeeProjects).FirstOrDefaultAsync(e => e.Id == id);
        if (employee == null) return "Employee not found.";

        // Зв'язки налаштовані як ClientSetNull, тож призначення видаляємо явно
        db.EmployeeProjects.RemoveRange(employee.EmployeeProjects);
        db.Employees.Remove(employee);
        await db.SaveChangesAsync();
        return null;
    }
}
