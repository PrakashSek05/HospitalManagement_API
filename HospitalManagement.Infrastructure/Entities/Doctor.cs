using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Entities;

public partial class Doctor
{
    [Key]
    public int DoctorId { get; set; }

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    public int DepartmentId { get; set; }

    public bool IsAvailable { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedUtc { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedUtc { get; set; }

    [InverseProperty("Doctor")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [ForeignKey("DepartmentId")]
    [InverseProperty("Doctors")]
    public virtual Department Department { get; set; } = null!;

    [InverseProperty("Doctor")]
    public virtual ICollection<PatientHistory> PatientHistories { get; set; } = new List<PatientHistory>();    
    public virtual ICollection<Specialty> Specialties { get; set; } = new List<Specialty>();
}
