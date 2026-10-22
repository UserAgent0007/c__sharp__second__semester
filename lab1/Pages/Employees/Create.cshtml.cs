using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;
using lab1.Models;

namespace lab1.Pages.Employees
{
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public CreateModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public EmployeeInput Input { get; set; } = new();

        // Список посад для <select>
        public SelectList Positions { get; set; } = new SelectList(Array.Empty<Position>());

        private async Task LoadPositionsAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");
            var list = await client.GetFromJsonAsync<List<Position>>("api/positions") ?? new();
            Positions = new SelectList(list, "Id", "Name", Input.PositionId);
        }

        public async Task OnGetAsync()
        {
            await LoadPositionsAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadPositionsAsync();
                return Page();
            }

            var client = _clientFactory.CreateClient("MyApi");
            var response = await client.PostAsJsonAsync("api/employees", Input);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Помилка при створенні працівника.");
            await LoadPositionsAsync();
            return Page();
        }
    }
}
