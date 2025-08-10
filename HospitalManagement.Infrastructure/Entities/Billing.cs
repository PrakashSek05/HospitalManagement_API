using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Entities;

[Table("Billing")]
public partial class Billing
{
    [Key]
    public int BillId { get; set; }

    public int PatientId { get; set; }

    public int? AppointmentId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Discount { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal Tax { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal NetAmount { get; set; }

    public bool PaidFlag { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedUtc { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("Billings")]
    public virtual Appointment? Appointment { get; set; }

    [InverseProperty("Bill")]
    public virtual ICollection<BillItem> BillItems { get; set; } = new List<BillItem>();

    [ForeignKey("PatientId")]
    [InverseProperty("Billings")]
    public virtual Patient Patient { get; set; } = null!;
}
