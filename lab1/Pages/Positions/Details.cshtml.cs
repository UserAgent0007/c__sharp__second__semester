using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using lab1.Models;

namespace lab1.Pages.Positions
{
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public DetailsModel(IHttpClientFactory clientFactory)
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
    }
}
