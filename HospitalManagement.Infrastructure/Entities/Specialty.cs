using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Entities;

[Index("SpecialtyName", Name = "UQ__Specialt__7DCA5748BFC136AC", IsUnique = true)]
public partial class Specialty
{
    [Key]
    public int SpecialtyId { get; set; }

    [StringLength(50)]
    public string SpecialtyName { get; set; } = null!;    
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
