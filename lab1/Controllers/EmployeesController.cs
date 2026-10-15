using lab1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab1.Controllers;

public class EmployeesController : Controller
{
    private readonly ProjectContractsContext _context;

    public EmployeesController(ProjectContractsContext context)
    {
        _context = context;
    }

    // GET: Employees
    public async Task<IActionResult> Index()
    {
        var employees = await _context.Employees
            .Include(e => e.Position)
            .Include(e => e.EmployeeProjects)
            .ToListAsync();
        return View(employees);
    }

    // GET: Employees/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var employee = await _context.Employees
            .Include(e => e.Position)
            .Include(e => e.EmployeeProjects)
            .ThenInclude(ep => ep.Project)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
            return NotFound();

        return View(employee);
    }

    // GET: Employees/Create
    public async Task<IActionResult> Create()
    {
        ViewBag.Positions = await _context.Positions.ToListAsync();
        return View();
    }

    // POST: Employees/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("FirstName,LastName,MiddleName,PositionId,Address,Characteristic")] Employee employee)
    {
        if (ModelState.IsValid)
        {
            _context.Add(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Positions = await _context.Positions.ToListAsync();
        return View(employee);
    }

    // GET: Employees/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return NotFound();

        ViewBag.Positions = await _context.Positions.ToListAsync();
        return View(employee);
    }

    // POST: Employees/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,MiddleName,PositionId,Address,Characteristic")] Employee employee)
    {
        if (id != employee.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(employee);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(employee.Id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Positions = await _context.Positions.ToListAsync();
        return View(employee);
    }

    // GET: Employees/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var employee = await _context.Employees
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
            return NotFound();

        return View(employee);
    }

    // POST: Employees/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee != null)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool EmployeeExists(int id) => _context.Employees.Any(e => e.Id == id);
}
