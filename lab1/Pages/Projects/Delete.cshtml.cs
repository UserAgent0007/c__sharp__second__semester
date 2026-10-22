using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using lab1.Models;

namespace lab1.Pages.Projects
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

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");

            var response = await client.DeleteAsync($"api/projects/{Id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Index");
            }

            ModelState.AddModelError(string.Empty, "Помилка при видаленні проєкту.");

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
