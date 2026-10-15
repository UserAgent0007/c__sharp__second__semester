using lab1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab1.Controllers;

public class EmployeeProjectsController : Controller
{
    private readonly ProjectContractsContext _context;

    public EmployeeProjectsController(ProjectContractsContext context)
    {
        _context = context;
    }

    // GET: EmployeeProjects/Assign/5 (Employee ID)
    public async Task<IActionResult> Assign(int? employeeId)
    {
        if (employeeId == null)
            return NotFound();

        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null)
            return NotFound();

        var assignedProjects = await _context.EmployeeProjects
            .Where(ep => ep.EmployeeId == employeeId)
            .Select(ep => ep.ProjectId)
            .ToListAsync();

        var availableProjects = await _context.Projects
            .Where(p => !assignedProjects.Contains(p.Id))
            .ToListAsync();

        ViewBag.EmployeeId = employeeId;
        ViewBag.EmployeeName = $"{employee.FirstName} {employee.LastName}";
        ViewBag.Projects = availableProjects;

        return View();
    }

    // POST: EmployeeProjects/Assign
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Assign(int employeeId, int projectId, DateOnly startDate, DateOnly endDate)
    {
        if (employeeId <= 0 || projectId <= 0)
            return BadRequest("Invalid employee or project ID");

        if (startDate > endDate)
            ModelState.AddModelError("", "Start date must be before end date");

        if (ModelState.IsValid)
        {
            var employeeProject = new EmployeeProject
            {
                EmployeeId = employeeId,
                ProjectId = projectId,
                StartDate = startDate,
                EndDate = endDate
            };

            _context.Add(employeeProject);
            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Employees", new { id = employeeId });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "This employee is already assigned to this project");
            }
        }

        var employee = await _context.Employees.FindAsync(employeeId);
        var assignedProjects = await _context.EmployeeProjects
            .Where(ep => ep.EmployeeId == employeeId)
            .Select(ep => ep.ProjectId)
            .ToListAsync();

        var availableProjects = await _context.Projects
            .Where(p => !assignedProjects.Contains(p.Id))
            .ToListAsync();

        ViewBag.EmployeeId = employeeId;
        ViewBag.EmployeeName = $"{employee.FirstName} {employee.LastName}";
        ViewBag.Projects = availableProjects;

        return View();
    }

    // GET: EmployeeProjects/Edit/5 (EmployeeProject ID)
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var employeeProject = await _context.EmployeeProjects
            .Include(ep => ep.Employee)
            .Include(ep => ep.Project)
            .FirstOrDefaultAsync(ep => ep.EmployeeProjectId == id);

        if (employeeProject == null)
            return NotFound();

        return View(employeeProject);
    }

    // POST: EmployeeProjects/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("EmployeeProjectId,EmployeeId,ProjectId,StartDate,EndDate")] EmployeeProject employeeProject)
    {
        if (id != employeeProject.EmployeeProjectId)
            return NotFound();

        if (employeeProject.StartDate > employeeProject.EndDate)
            ModelState.AddModelError("", "Start date must be before end date");

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(employeeProject);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Employees", new { id = employeeProject.EmployeeId });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeProjectExists(employeeProject.EmployeeProjectId))
                    return NotFound();
                throw;
            }
        }

        return View(employeeProject);
    }

    // GET: EmployeeProjects/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var employeeProject = await _context.EmployeeProjects
            .Include(ep => ep.Employee)
            .Include(ep => ep.Project)
            .FirstOrDefaultAsync(ep => ep.EmployeeProjectId == id);

        if (employeeProject == null)
            return NotFound();

        return View(employeeProject);
    }

    // POST: EmployeeProjects/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employeeProject = await _context.EmployeeProjects.FindAsync(id);
        int? employeeId = employeeProject?.EmployeeId;

        if (employeeProject != null)
        {
            _context.EmployeeProjects.Remove(employeeProject);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Details", "Employees", new { id = employeeId });
    }

    private bool EmployeeProjectExists(int id) => 
        _context.EmployeeProjects.Any(e => e.EmployeeProjectId == id);
}
