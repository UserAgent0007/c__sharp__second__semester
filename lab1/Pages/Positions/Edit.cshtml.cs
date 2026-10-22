using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using lab1.Models;

namespace lab1.Pages.Positions
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public EditModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        [BindProperty]
        public PositionInput Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");

            var position = await client.GetOrNullAsync<Position>($"api/positions/{Id}");
            if (position == null)
            {
                return NotFound();
            }

            Input = new PositionInput { Name = position.Name, Salary = position.Salary };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _clientFactory.CreateClient("MyApi");

            var payload = new { Id = Id, Input.Name, Input.Salary };
            var response = await client.PutAsJsonAsync($"api/positions/{Id}", payload);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Details", new { id = Id });
            }

            ModelState.AddModelError(string.Empty, "Помилка при збереженні змін.");
            return Page();
        }
    }
}
