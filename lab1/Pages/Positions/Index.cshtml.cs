using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using lab1.Models;

namespace lab1.Pages.Positions
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<Position> Positions { get; set; } = new();

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");
            Positions = await client.GetFromJsonAsync<List<Position>>("api/positions") ?? new();
        }
    }
}
