using lab1.Api.Dtos;
using lab1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
    private readonly ProjectContractsContext _context;

    public PositionsController(ProjectContractsContext context)
    {
        _context = context;
    }

    // GET api/positions
    [HttpGet]
    public async Task<ActionResult<List<Position>>> GetAll()
    {
        return await _context.Positions
            .AsNoTracking()
            .Include(p => p.Employees)
            .ToListAsync();
    }

    // GET api/positions/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Position>> GetById(int id)
    {
        var position = await _context.Positions
            .AsNoTracking()
            .Include(p => p.Employees)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (position == null)
            return NotFound();

        return position;
    }

    // POST api/positions
    [HttpPost]
    public async Task<ActionResult<Position>> Create(PositionRequest request)
    {
        var position = new Position { Name = request.Name, Salary = request.Salary };

        _context.Positions.Add(position);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = position.Id }, position);
    }

    // PUT api/positions/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PositionRequest request)
    {
        var position = await _context.Positions.FindAsync(id);
        if (position == null)
            return NotFound();

        position.Name = request.Name;
        position.Salary = request.Salary;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/positions/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var position = await _context.Positions
            .Include(p => p.Employees)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (position == null)
            return NotFound();

        if (position.Employees.Any())
            return Conflict(new { message = "Неможливо видалити посаду, оскільки до неї прив'язані співробітники." });

        _context.Positions.Remove(position);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
