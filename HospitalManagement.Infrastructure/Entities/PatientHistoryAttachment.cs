using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagement.Infrastructure.Entities;

[Index("HistoryId", Name = "IX_PatientHistoryAttachments_HistoryId")]
public partial class PatientHistoryAttachment
{
    [Key]
    public int AttachmentId { get; set; }

    public int HistoryId { get; set; }

    [StringLength(200)]
    public string FileName { get; set; } = null!;

    [StringLength(400)]
    public string FilePath { get; set; } = null!;

    [StringLength(100)]
    public string? ContentType { get; set; }

    public long? FileSizeBytes { get; set; }

    [StringLength(200)]
    public string? Notes { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime UploadedUtc { get; set; }

    [ForeignKey("HistoryId")]
    [InverseProperty("PatientHistoryAttachments")]
    public virtual PatientHistory History { get; set; } = null!;
}
