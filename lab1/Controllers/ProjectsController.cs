using lab1.Api.Dtos;
using lab1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly ProjectContractsContext _context;

    public ProjectsController(ProjectContractsContext context)
    {
        _context = context;
    }

    // GET api/projects
    [HttpGet]
    public async Task<ActionResult<List<Project>>> GetAll()
    {
        return await _context.Projects
            .AsNoTracking()
            .Include(p => p.EmployeeProjects)
            .ToListAsync();
    }

    // GET api/projects/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Project>> GetById(int id)
    {
        var project = await _context.Projects
            .AsNoTracking()
            .Include(p => p.EmployeeProjects)
            .ThenInclude(ep => ep.Employee)
            .ThenInclude(e => e.Position)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound();

        return project;
    }

    // POST api/projects
    [HttpPost]
    public async Task<ActionResult<Project>> Create(ProjectRequest request)
    {
        var project = new Project { Name = request.Name, Description = request.Description };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    // PUT api/projects/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProjectRequest request)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null)
            return NotFound();

        project.Name = request.Name;
        project.Description = request.Description;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/projects/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _context.Projects
            .Include(p => p.EmployeeProjects)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null)
            return NotFound();

        // Сторінка видалення обіцяє "Deleting it will remove all assignments",
        // тому призначення видаляємо разом із проєктом.
        _context.EmployeeProjects.RemoveRange(project.EmployeeProjects);
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
