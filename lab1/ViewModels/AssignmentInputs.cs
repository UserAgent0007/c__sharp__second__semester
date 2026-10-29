using System.ComponentModel.DataAnnotations;

namespace lab1.ViewModels;

// Форма призначення працівника на проєкт
public class AssignInput : IValidatableObject
{
    [Required(ErrorMessage = "Select a project.")]
    public int? ProjectId { get; set; }

    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate < StartDate)
            yield return new ValidationResult("End date must be after start date.", new[] { nameof(EndDate) });
    }
}

// Форма редагування призначення: змінюються лише дати
public class AssignmentDatesInput : IValidatableObject
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate < StartDate)
            yield return new ValidationResult("End date must be after start date.", new[] { nameof(EndDate) });
    }
}
