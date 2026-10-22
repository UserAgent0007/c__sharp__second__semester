using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using lab1.Models;

namespace lab1.Pages.EmployeeProjects
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public EditModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // Id запису EmployeeProject, береться з маршруту {id:int}
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        // Дані лише для відображення (з форми не приходять)
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;

        [BindProperty]
        public EditInputModel Input { get; set; } = new();

        public class EditInputModel
        {
            public DateOnly StartDate { get; set; }
            public DateOnly EndDate { get; set; }
        }

        // Завантажує запис з API і заповнює властивості для відображення.
        // Input тут НЕ чіпаємо, щоб при помилці POST не втратити введені дані.
        private async Task<EmployeeProject?> LoadAssignmentAsync(HttpClient client)
        {
            var assignment = await client.GetOrNullAsync<EmployeeProject>($"api/employeeprojects/{Id}");
            if (assignment == null)
            {
                return null;
            }

            EmployeeId = assignment.EmployeeId;
            EmployeeName = $"{assignment.Employee?.FirstName} {assignment.Employee?.LastName}".Trim();
            ProjectName = assignment.Project?.Name ?? string.Empty;

            return assignment;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");

            var assignment = await LoadAssignmentAsync(client);
            if (assignment == null)
            {
                return NotFound();
            }

            // Підставляємо поточні дати у форму
            Input.StartDate = assignment.StartDate;
            Input.EndDate = assignment.EndDate;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");

            var assignment = await LoadAssignmentAsync(client);
            if (assignment == null)
            {
                return NotFound();
            }

            if (Input.EndDate < Input.StartDate)
            {
                ModelState.AddModelError("Input.EndDate", "End date must be after start date.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // EmployeeId і ProjectId беремо із завантаженого запису, а не з форми
            var payload = new
            {
                EmployeeProjectId = Id,
                EmployeeId = assignment.EmployeeId,
                ProjectId = assignment.ProjectId,
                StartDate = Input.StartDate,
                EndDate = Input.EndDate
            };

            var response = await client.PutAsJsonAsync($"api/employeeprojects/{Id}", payload);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Employees/Details", new { id = assignment.EmployeeId });
            }

            ModelState.AddModelError(string.Empty, "Помилка при збереженні змін.");
            return Page();
        }
    }
}
