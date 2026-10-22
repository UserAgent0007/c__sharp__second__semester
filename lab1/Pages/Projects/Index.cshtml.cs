using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using lab1.Models;

namespace lab1.Pages.Projects
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<Project> Projects { get; set; } = new();

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");
            Projects = await client.GetFromJsonAsync<List<Project>>("api/projects") ?? new();
        }
    }
}
