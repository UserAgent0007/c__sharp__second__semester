using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;
using lab1.Models;

namespace lab1.Pages.Projects
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
        public ProjectInput Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _clientFactory.CreateClient("MyApi");

            var project = await client.GetOrNullAsync<Project>($"api/projects/{Id}");
            if (project == null)
            {
                return NotFound();
            }

            Input = new ProjectInput { Name = project.Name, Description = project.Description };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _clientFactory.CreateClient("MyApi");

            var payload = new { Id = Id, Input.Name, Input.Description };
            var response = await client.PutAsJsonAsync($"api/projects/{Id}", payload);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("Details", new { id = Id });
            }

            ModelState.AddModelError(string.Empty, "Помилка при збереженні змін.");
            return Page();
        }
    }
}
