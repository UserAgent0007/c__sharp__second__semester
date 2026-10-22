using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using lab1.Models;

namespace lab1.Pages.Employees
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<Employee> Employees { get; set; } = new();

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");
            Employees = await client.GetFromJsonAsync<List<Employee>>("api/employees") ?? new();
        }
    }
}
