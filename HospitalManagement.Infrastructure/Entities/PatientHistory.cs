using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Entities;

[Table("PatientHistory")]
public partial class PatientHistory
{
    [Key]
    public int HistoryId { get; set; }

    public int PatientId { get; set; }

    public int? AppointmentId { get; set; }

    public int DoctorId { get; set; }

    [StringLength(20)]
    public string Type { get; set; } = null!;

    [StringLength(200)]
    public string Summary { get; set; } = null!;

    public string? Details { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedUtc { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("PatientHistories")]
    public virtual Appointment? Appointment { get; set; }

    [ForeignKey("DoctorId")]
    [InverseProperty("PatientHistories")]
    public virtual Doctor Doctor { get; set; } = null!;

    [ForeignKey("PatientId")]
    [InverseProperty("PatientHistories")]
    public virtual Patient Patient { get; set; } = null!;

    [InverseProperty("History")]
    public virtual ICollection<PatientHistoryAttachment> PatientHistoryAttachments { get; set; } = new List<PatientHistoryAttachment>();
}
