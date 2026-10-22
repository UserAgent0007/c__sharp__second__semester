using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using lab1.Models;

namespace lab1.Pages.Positions
{
    public class DeleteModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public DeleteModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public Position Position { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");

            var position = await client.GetOrNullAsync<Position>($"api/positions/{Id}");
            if (position == null)
            {
                return NotFound();
            }

            Position = position;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");

            var position = await client.GetOrNullAsync<Position>($"api/positions/{Id}");
            if (position == null)
            {
                return NotFound();
            }
            Position = position;

            // Серверна перевірка: кнопка в HTML лише disabled, її легко обійти
            if (position.Employees.Any())
            {
                ModelState.AddModelError(string.Empty, "Cannot delete a position that still has employees.");
                return Page();
            }

            var response = await client.DeleteAsync($"api/positions/{Id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Помилка при видаленні посади.");
            return Page();
        }
    }
}
