using System.ComponentModel.DataAnnotations;

namespace lab1.ViewModels;

// Поля форми створення/редагування працівника (спільна модель для Create і Edit)
public class EmployeeInput
{
    [Required, StringLength(50), Display(Name = "First name")]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50), Display(Name = "Last name")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(50), Display(Name = "Middle name")]
    public string? MiddleName { get; set; }

    [Required(ErrorMessage = "Select a position.")]
    [Display(Name = "Position")]
    public int? PositionId { get; set; }

    [Required, StringLength(200)]
    public string Address { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Characteristic { get; set; }
}
