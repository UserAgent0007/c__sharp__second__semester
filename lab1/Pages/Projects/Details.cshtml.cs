using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using lab1.Models;

namespace lab1.Pages.Projects
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

        public Project Project { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");

            var project = await client.GetOrNullAsync<Project>($"api/projects/{Id}");
            if (project == null)
            {
                return NotFound();
            }

            Project = project;
            return Page();
        }
    }
}
