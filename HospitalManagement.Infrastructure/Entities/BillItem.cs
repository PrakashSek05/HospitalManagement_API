using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Entities;

public partial class BillItem
{
    [Key]
    public int BillItemId { get; set; }

    public int BillId { get; set; }

    [StringLength(100)]
    public string Description { get; set; } = null!;

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(21, 2)")]
    public decimal? TotalPrice { get; set; }

    [ForeignKey("BillId")]
    [InverseProperty("BillItems")]
    public virtual Billing Bill { get; set; } = null!;
}
