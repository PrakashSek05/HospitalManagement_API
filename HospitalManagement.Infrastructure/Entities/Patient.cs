using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Entities;

[Index("Mrn", Name = "UQ__Patients__C790FDB42AA83E90", IsUnique = true)]
public partial class Patient
{
    [Key]
    public int PatientId { get; set; }

    [Column("MRN")]
    [StringLength(20)]
    public string Mrn { get; set; } = null!;

    [StringLength(100)]
    public string FullName { get; set; } = null!;

    [Column("DOB")]
    public DateOnly? Dob { get; set; }

    [StringLength(10)]
    public string? Gender { get; set; }

    [StringLength(15)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    public bool Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedUtc { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedUtc { get; set; }

    [InverseProperty("Patient")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("Patient")]
    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    [InverseProperty("Patient")]
    public virtual ICollection<PatientHistory> PatientHistories { get; set; } = new List<PatientHistory>();
}
