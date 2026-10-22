using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;
using lab1.Models;

namespace lab1.Pages.Employees
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public EditModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        // Id працівника, береться з маршруту {id:int}
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public EmployeeInput Input { get; set; } = new();

        public SelectList Positions { get; set; } = new SelectList(Array.Empty<Position>());

        private async Task LoadPositionsAsync(HttpClient client)
        {
            var list = await client.GetFromJsonAsync<List<Position>>("api/positions") ?? new();
            Positions = new SelectList(list, "Id", "Name", Input.PositionId);
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");

            var employee = await client.GetOrNullAsync<Employee>($"api/employees/{Id}");
            if (employee == null)
            {
                return NotFound();
            }

            Input = new EmployeeInput
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                MiddleName = employee.MiddleName,
                PositionId = employee.PositionId,
                Address = employee.Address,
                Characteristic = employee.Characteristic
            };

            await LoadPositionsAsync(client);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");

            if (!ModelState.IsValid)
            {
                await LoadPositionsAsync(client);
                return Page();
            }

            var payload = new
            {
                Id = Id,
                Input.FirstName,
                Input.LastName,
                Input.MiddleName,
                Input.PositionId,
                Input.Address,
                Input.Characteristic
            };

            var response = await client.PutAsJsonAsync($"api/employees/{Id}", payload);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Details", new { id = Id });
            }

            ModelState.AddModelError(string.Empty, "Помилка при збереженні змін.");
            await LoadPositionsAsync(client);
            return Page();
        }
    }
}
