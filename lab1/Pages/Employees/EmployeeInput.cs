using System.ComponentModel.DataAnnotations;

namespace lab1.Pages.Employees;

// Поля форми створення/редагування працівника (спільна модель для Create і Edit)
public class EmployeeInput
{
    [Required, Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Middle name")]
    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Select a position.")]
    [Display(Name = "Position")]
    public int? PositionId { get; set; }

    [Required]
    public string Address { get; set; } = string.Empty;

    public string? Characteristic { get; set; }
}
