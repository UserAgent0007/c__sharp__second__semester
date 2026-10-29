using lab1.Models;
using lab1.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace lab1.Services;

public class EmployeeProjectService(IDbContextFactory<ProjectContractsContext> factory)
{
    public async Task<EmployeeProject?> GetByIdAsync(int id)
    {
        await using var db = factory.CreateDbContext();
        return await db.EmployeeProjects.AsNoTracking()
            .Include(ep => ep.Employee)
            .Include(ep => ep.Project)
            .FirstOrDefaultAsync(ep => ep.EmployeeProjectId == id);
    }

    // Проєкти, на які цей працівник ще не призначений
    public async Task<List<Project>> GetAvailableProjectsAsync(int employeeId)
    {
        await using var db = factory.CreateDbContext();
        var assignedIds = db.EmployeeProjects
            .Where(ep => ep.EmployeeId == employeeId)
            .Select(ep => ep.ProjectId);

        return await db.Projects.AsNoTracking()
            .Where(p => !assignedIds.Contains(p.Id))
            .ToListAsync();
    }

    public async Task<string?> AssignAsync(int employeeId, AssignInput input)
    {
        if (input.EndDate < input.StartDate)
            return "End date must be after start date.";

        if (input.ProjectId is not int projectId)
            return "Select a project.";

        await using var db = factory.CreateDbContext();

        if (!await db.Employees.AnyAsync(e => e.Id == employeeId) ||
            !await db.Projects.AnyAsync(p => p.Id == projectId))
            return "Employee or project not found.";

        // В базі є унікальний індекс (EmployeeId, ProjectId)
        if (await db.EmployeeProjects.AnyAsync(ep => ep.EmployeeId == employeeId && ep.ProjectId == projectId))
            return "This employee is already assigned to this project.";

        db.EmployeeProjects.Add(new EmployeeProject
        {
            EmployeeId = employeeId,
            ProjectId = projectId,
            StartDate = input.StartDate,
            EndDate = input.EndDate
        });
        await db.SaveChangesAsync();
        return null;
    }

    public async Task<string?> UpdateAsync(int id, AssignmentDatesInput input)
    {
        if (input.EndDate < input.StartDate)
            return "End date must be after start date.";

        await using var db = factory.CreateDbContext();
        var assignment = await db.EmployeeProjects.FindAsync(id);
        if (assignment == null) return "Assignment not found.";

        assignment.StartDate = input.StartDate;
        assignment.EndDate = input.EndDate;
        await db.SaveChangesAsync();
        return null;
    }

    public async Task<string?> DeleteAsync(int id)
    {
        await using var db = factory.CreateDbContext();
        var assignment = await db.EmployeeProjects.FindAsync(id);
        if (assignment == null) return "Assignment not found.";

        db.EmployeeProjects.Remove(assignment);
        await db.SaveChangesAsync();
        return null;
    }
}
