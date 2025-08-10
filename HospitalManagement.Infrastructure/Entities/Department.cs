using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Entities;

public partial class Department
{
    [Key]
    public int DepartmentId { get; set; }

    [StringLength(50)]
    public string DepartmentName { get; set; } = null!;

    [StringLength(200)]
    public string? Description { get; set; }

    [InverseProperty("Department")]
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
