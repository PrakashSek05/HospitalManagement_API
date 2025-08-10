using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Entities;

public partial class Appointment
{
    [Key]
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime VisitDateTime { get; set; }

    [StringLength(200)]
    public string? Reason { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedUtc { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedUtc { get; set; }

    [InverseProperty("Appointment")]
    public virtual ICollection<Billing> Billings { get; set; } = new List<Billing>();

    [ForeignKey("DoctorId")]
    [InverseProperty("Appointments")]
    public virtual Doctor Doctor { get; set; } = null!;

    [ForeignKey("PatientId")]
    [InverseProperty("Appointments")]
    public virtual Patient Patient { get; set; } = null!;

    [InverseProperty("Appointment")]
    public virtual ICollection<PatientHistory> PatientHistories { get; set; } = new List<PatientHistory>();
}
