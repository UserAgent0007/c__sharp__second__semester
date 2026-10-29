using System.ComponentModel.DataAnnotations;

namespace lab1.ViewModels;

public class ProjectInput
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
