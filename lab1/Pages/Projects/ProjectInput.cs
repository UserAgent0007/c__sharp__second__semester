using System.ComponentModel.DataAnnotations;

namespace lab1.Pages.Projects;

public class ProjectInput
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
