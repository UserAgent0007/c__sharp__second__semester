using lab1.Models;
using lab1.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace lab1.Services;

public class ProjectService(IDbContextFactory<ProjectContractsContext> factory)
{
    public async Task<List<Project>> GetAllAsync()
    {
        await using var db = factory.CreateDbContext();
        return await db.Projects.AsNoTracking().Include(p => p.EmployeeProjects).ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        await using var db = factory.CreateDbContext();
        return await db.Projects.AsNoTracking()
            .Include(p => p.EmployeeProjects)
                .ThenInclude(ep => ep.Employee)
                    .ThenInclude(e => e.Position)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task CreateAsync(ProjectInput input)
    {
        await using var db = factory.CreateDbContext();
        db.Projects.Add(new Project { Name = input.Name, Description = input.Description });
        await db.SaveChangesAsync();
    }

    public async Task<string?> UpdateAsync(int id, ProjectInput input)
    {
        await using var db = factory.CreateDbContext();
        var project = await db.Projects.FindAsync(id);
        if (project == null) return "Project not found.";

        project.Name = input.Name;
        project.Description = input.Description;
        await db.SaveChangesAsync();
        return null;
    }

    public async Task<string?> DeleteAsync(int id)
    {
        await using var db = factory.CreateDbContext();
        var project = await db.Projects.Include(p => p.EmployeeProjects).FirstOrDefaultAsync(p => p.Id == id);
        if (project == null) return "Project not found.";

        // Разом із проєктом видаляються всі його призначення
        db.EmployeeProjects.RemoveRange(project.EmployeeProjects);
        db.Projects.Remove(project);
        await db.SaveChangesAsync();
        return null;
    }
}
