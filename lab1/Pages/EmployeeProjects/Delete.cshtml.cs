using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using lab1.Models;

namespace lab1.Pages.EmployeeProjects
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public DeleteModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // Id запису EmployeeProject, береться з маршруту {id:int}
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        // Дані для відображення на сторінці підтвердження
        public EmployeeProject Assignment { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");

            var assignment = await client.GetOrNullAsync<EmployeeProject>($"api/employeeprojects/{Id}");
            if (assignment == null)
            {
                return NotFound();
            }

            Assignment = assignment;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");

            // Спочатку отримуємо запис, щоб знати EmployeeId для редиректу
            var assignment = await client.GetOrNullAsync<EmployeeProject>($"api/employeeprojects/{Id}");
            if (assignment == null)
            {
                return NotFound();
            }

            var response = await client.DeleteAsync($"api/employeeprojects/{Id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Employees/Details", new { id = assignment.EmployeeId });
            }

            ModelState.AddModelError(string.Empty, "Помилка при видаленні призначення.");
            Assignment = assignment;
            return Page();
        }
    }
}
