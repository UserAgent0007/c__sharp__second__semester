using System.ComponentModel.DataAnnotations;

namespace lab1.Api.Dtos;

// Моделі запитів. Приймаємо лише потрібні поля, а не цілі сутності:
// так клієнту не треба передавати навігаційні властивості (Position тощо),
// і він не може "підмінити" зайві поля (over-posting).

public class EmployeeRequest
{
    [Required, StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [StringLength(50)]
    public string? MiddleName { get; set; }

    [Range(1, int.MaxValue)]
    public int PositionId { get; set; }

    [Required, StringLength(200)]
    public string Address { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Characteristic { get; set; }
}

public class PositionRequest
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Range(0, 99999999.99)]
    public decimal Salary { get; set; }
}

public class ProjectRequest
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}

public class AssignRequest
{
    [Range(1, int.MaxValue)]
    public int EmployeeId { get; set; }

    [Range(1, int.MaxValue)]
    public int ProjectId { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}

// При редагуванні змінюються лише дати
public class UpdateAssignmentRequest
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}
