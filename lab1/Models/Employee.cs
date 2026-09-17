using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace lab1.Models;

[Index("PositionId", Name = "IX_Employees_PositionId")]
public partial class Employee
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    public string? MiddleName { get; set; }

    public int PositionId { get; set; }

    [StringLength(200)]
    public string Address { get; set; } = null!;

    [StringLength(500)]
    public string? Characteristic { get; set; }

    [InverseProperty("Employee")]
    public virtual ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();

    [ForeignKey("PositionId")]
    [InverseProperty("Employees")]
    public virtual Position Position { get; set; } = null!;
}
