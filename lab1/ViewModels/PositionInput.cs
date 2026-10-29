using System.ComponentModel.DataAnnotations;

namespace lab1.ViewModels;

public class PositionInput
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 99999999.99)]
    public decimal Salary { get; set; }
}
