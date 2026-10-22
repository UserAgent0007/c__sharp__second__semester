using lab1.Api.Dtos;
using lab1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeProjectsController : ControllerBase
{
    private readonly ProjectContractsContext _context;

    public EmployeeProjectsController(ProjectContractsContext context)
    {
        _context = context;
    }

    // GET api/employeeprojects/5 (EmployeeProjectId)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeProject>> GetById(int id)
    {
        var assignment = await _context.EmployeeProjects
            .AsNoTracking()
            .Include(ep => ep.Employee)
            .Include(ep => ep.Project)
            .FirstOrDefaultAsync(ep => ep.EmployeeProjectId == id);

        if (assignment == null)
            return NotFound();

        return assignment;
    }

    // GET api/employeeprojects/available/5 (EmployeeId)
    // Проєкти, на які цей працівник ще не призначений
    [HttpGet("available/{employeeId:int}")]
    public async Task<ActionResult<List<Project>>> GetAvailable(int employeeId)
    {
        if (!await _context.Employees.AnyAsync(e => e.Id == employeeId))
            return NotFound();

        var assignedProjectIds = _context.EmployeeProjects
            .Where(ep => ep.EmployeeId == employeeId)
            .Select(ep => ep.ProjectId);

        return await _context.Projects
            .AsNoTracking()
            .Where(p => !assignedProjectIds.Contains(p.Id))
            .ToListAsync();
    }

    // POST api/employeeprojects/assign
    [HttpPost("assign")]
    public async Task<ActionResult<EmployeeProject>> Assign(AssignRequest request)
    {
        if (request.StartDate > request.EndDate)
        {
            ModelState.AddModelError(nameof(request.EndDate), "Start date must be before end date.");
            return ValidationProblem(ModelState);
        }

        if (!await _context.Employees.AnyAsync(e => e.Id == request.EmployeeId) ||
            !await _context.Projects.AnyAsync(p => p.Id == request.ProjectId))
        {
            return NotFound(new { message = "Employee or project not found." });
        }

        // В базі є унікальний індекс (EmployeeId, ProjectId)
        if (await _context.EmployeeProjects.AnyAsync(ep =>
                ep.EmployeeId == request.EmployeeId && ep.ProjectId == request.ProjectId))
        {
            return Conflict(new { message = "This employee is already assigned to this project." });
        }

        var assignment = new EmployeeProject
        {
            EmployeeId = request.EmployeeId,
            ProjectId = request.ProjectId,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        _context.EmployeeProjects.Add(assignment);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = assignment.EmployeeProjectId }, assignment);
    }

    // PUT api/employeeprojects/5
    // Змінюються лише дати, працівник і проєкт залишаються тими самими
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateAssignmentRequest request)
    {
        if (request.StartDate > request.EndDate)
        {
            ModelState.AddModelError(nameof(request.EndDate), "Start date must be before end date.");
            return ValidationProblem(ModelState); // ProblemDetails with 400 Bad Request
        }

        var assignment = await _context.EmployeeProjects.FindAsync(id);
        if (assignment == null)
            return NotFound();

        assignment.StartDate = request.StartDate;
        assignment.EndDate = request.EndDate;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/employeeprojects/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var assignment = await _context.EmployeeProjects.FindAsync(id);
        if (assignment == null)
            return NotFound();

        _context.EmployeeProjects.Remove(assignment);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
