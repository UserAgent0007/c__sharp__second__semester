using System.ComponentModel.DataAnnotations;

namespace lab1.Pages.Positions;

public class PositionInput
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Range(0, 1000000000)]
    public decimal Salary { get; set; }
}
