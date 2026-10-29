using lab1.Models;
using lab1.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace lab1.Services;

// Методи, що повертають string?, повертають null при успіху або текст помилки.
public class PositionService(IDbContextFactory<ProjectContractsContext> factory)
{
    // Із кількістю працівників (для списку)
    public async Task<List<Position>> GetAllAsync()
    {
        await using var db = factory.CreateDbContext();
        return await db.Positions.AsNoTracking().Include(p => p.Employees).ToListAsync();
    }

    // Без працівників (для випадаючих списків)
    public async Task<List<Position>> GetListAsync()
    {
        await using var db = factory.CreateDbContext();
        return await db.Positions.AsNoTracking().ToListAsync();
    }

    public async Task<Position?> GetByIdAsync(int id)
    {
        await using var db = factory.CreateDbContext();
        return await db.Positions.AsNoTracking()
            .Include(p => p.Employees)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task CreateAsync(PositionInput input)
    {
        await using var db = factory.CreateDbContext();
        db.Positions.Add(new Position { Name = input.Name, Salary = input.Salary });
        await db.SaveChangesAsync();
    }

    public async Task<string?> UpdateAsync(int id, PositionInput input)
    {
        await using var db = factory.CreateDbContext();
        var position = await db.Positions.FindAsync(id);
        if (position == null) return "Position not found.";

        position.Name = input.Name;
        position.Salary = input.Salary;
        await db.SaveChangesAsync();
        return null;
    }

    public async Task<string?> DeleteAsync(int id)
    {
        await using var db = factory.CreateDbContext();
        var position = await db.Positions.Include(p => p.Employees).FirstOrDefaultAsync(p => p.Id == id);
        if (position == null) return "Position not found.";

        if (position.Employees.Any())
            return "Cannot delete a position that still has employees.";

        db.Positions.Remove(position);
        await db.SaveChangesAsync();
        return null;
    }
}
