using lab1.Api.Dtos;
using lab1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly ProjectContractsContext _context;

    public EmployeesController(ProjectContractsContext context)
    {
        _context = context;
    }

    // GET api/employees
    [HttpGet]
    public async Task<ActionResult<List<Employee>>> GetAll()
    {
        var data = await _context.Employees
            .AsNoTracking()
            .Include(e => e.Position)
            .Include(e => e.EmployeeProjects)
            .ToListAsync();
        return data;
    }

    // GET api/employees/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Employee>> GetById(int id)
    {
        var employee = await _context.Employees
            .AsNoTracking()
            .Include(e => e.Position)
            .Include(e => e.EmployeeProjects)
            .ThenInclude(ep => ep.Project)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
            return NotFound();

        return employee;
    }

    // POST api/employees
    [HttpPost]
    public async Task<ActionResult<Employee>> Create(EmployeeRequest request)
    {
        if (!await _context.Positions.AnyAsync(p => p.Id == request.PositionId))
        {
            ModelState.AddModelError(nameof(request.PositionId), "Position does not exist.");
            return ValidationProblem(ModelState);
        }

        var employee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            MiddleName = request.MiddleName,
            PositionId = request.PositionId,
            Address = request.Address,
            Characteristic = request.Characteristic
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    // PUT api/employees/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, EmployeeRequest request)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
            return NotFound();

        if (!await _context.Positions.AnyAsync(p => p.Id == request.PositionId))
        {
            ModelState.AddModelError(nameof(request.PositionId), "Position does not exist.");
            return ValidationProblem(ModelState);
        }

        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.MiddleName = request.MiddleName;
        employee.PositionId = request.PositionId;
        employee.Address = request.Address;
        employee.Characteristic = request.Characteristic;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/employees/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var employee = await _context.Employees
            .Include(e => e.EmployeeProjects)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
            return NotFound();

        // Зв'язки налаштовані як ClientSetNull, тож призначення на проєкти
        // потрібно видалити явно, інакше база відхилить видалення через FK.
        _context.EmployeeProjects.RemoveRange(employee.EmployeeProjects);
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
