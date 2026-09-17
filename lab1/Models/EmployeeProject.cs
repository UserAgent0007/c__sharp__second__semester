using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace lab1.Models;

[Index("EmployeeId", Name = "IX_EmployeeProjects_EmployeeId")]
[Index("ProjectId", Name = "IX_EmployeeProjects_ProjectId")]
[Index("EmployeeId", "ProjectId", Name = "UQ_EmployeeProjects", IsUnique = true)]
public partial class EmployeeProject
{
    [Key]
    public int EmployeeProjectId { get; set; }

    public int EmployeeId { get; set; }

    public int ProjectId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("EmployeeProjects")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("ProjectId")]
    [InverseProperty("EmployeeProjects")]
    public virtual Project Project { get; set; } = null!;
}
