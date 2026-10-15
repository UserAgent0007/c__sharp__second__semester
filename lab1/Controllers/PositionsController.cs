using lab1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace lab1.Controllers;

public class PositionsController : Controller
{
    private readonly ProjectContractsContext _context;

    public PositionsController(ProjectContractsContext context)
    {
        _context = context;
    }

    // GET: Positions
    public async Task<IActionResult> Index()
    {
        var positions = await _context.Positions
            .Include(p => p.Employees)
            .ToListAsync();
        return View(positions);
    }

    // GET: Positions/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var position = await _context.Positions
            .Include(p => p.Employees)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (position == null)
            return NotFound();

        return View(position);
    }

    // GET: Positions/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Positions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Salary")] Position position)
    {
        if (ModelState.IsValid)
        {
            _context.Add(position);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(position);
    }

    // GET: Positions/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var position = await _context.Positions.FindAsync(id);
        if (position == null)
            return NotFound();

        return View(position);
    }

    // POST: Positions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Salary")] Position position)
    {
        if (id != position.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(position);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PositionExists(position.Id))
                    return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        return View(position);
    }

    // GET: Positions/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var position = await _context.Positions.FindAsync(id);
        if (position == null)
            return NotFound();

        return View(position);
    }

    // POST: Positions/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var position = await _context.Positions.FindAsync(id);
        if (position != null)
        {
            try
            {
                _context.Positions.Remove(position);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                // Помилка виникає, якщо є пов'язані записи (наприклад, співробітники з цією посадою)
                ModelState.AddModelError("", "Неможливо видалити посаду, оскільки до неї прив'язані співробітники.");
                return View(position);
            }
            catch (Exception ex)
            {
                // Обробка будь-яких інших непередбачуваних помилок
                ModelState.AddModelError("", $"Сталася помилка при видаленні: {ex.Message}");
                return View(position);
            }
        }

        return RedirectToAction(nameof(Index));
    }

    private bool PositionExists(int id) => _context.Positions.Any(p => p.Id == id);
}
