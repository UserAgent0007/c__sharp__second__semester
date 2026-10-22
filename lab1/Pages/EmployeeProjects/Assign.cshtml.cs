using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using lab1.Models;

namespace lab1.Pages.EmployeeProjects
{
    public class AssignModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public AssignModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public List<Project> AvailableProjects { get; set; } = new();

        [BindProperty]
        public AssignInputModel Input { get; set; } = new();

        public class AssignInputModel
        {
            public int ProjectId { get; set; }
            public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
            public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");

            // Отримуємо дані працівника
            var employee = await client.GetOrNullAsync<Employee>($"api/employees/{EmployeeId}");
            if (employee == null)
            {
                return NotFound();
            }
            EmployeeName = $"{employee.FirstName} {employee.LastName}";

            // Отримуємо список доступних проєктів для цього працівника з API
            AvailableProjects = await client.GetFromJsonAsync<List<Project>>($"api/employeeprojects/available/{EmployeeId}")
                                ?? new List<Project>();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Input.EndDate < Input.StartDate)
            {
                ModelState.AddModelError("Input.EndDate", "End date must be after start date.");
            }

            if (!ModelState.IsValid)
            {
                // У разі помилки валідації повторно завантажуємо дані сторінки
                var result = await OnGetAsync();
                return result is NotFoundResult ? result : Page();
            }

            var client = _clientFactory.CreateClient("MyApi");

            var payload = new
            {
                EmployeeId = EmployeeId,
                ProjectId = Input.ProjectId,
                StartDate = Input.StartDate,
                EndDate = Input.EndDate
            };

            // Надсилаємо POST-запит до Web API
            var response = await client.PostAsJsonAsync("api/employeeprojects/assign", payload);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("/Employees/Details", new { id = EmployeeId });
            }

            ModelState.AddModelError(string.Empty, "Помилка при призначенні проєкту.");
            var reload = await OnGetAsync();
            return reload is NotFoundResult ? reload : Page();
        }
    }
}