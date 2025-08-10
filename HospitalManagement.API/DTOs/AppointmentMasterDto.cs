namespace HospitalManagement.API.Dtos
{
    public class AppointmentMasterDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = "";
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime VisitDateTime { get; set; }    
        public DateTime? LastVisitDateTime { get; set; }
        public string? LastDoctorName { get; set; }  
        public bool? IsEdit { get; set; }  
    }

    public class AppointmentDetailDto : AppointmentMasterDto
    {             
        public string? Type { get; set; }
        public string? Reason { get; set; }
        public string? Details { get; set; }

        public List<AttachmentDto> Attachments { get; set; } = new();
    }

    public class AttachmentDto
    {
        public int AttachmentId { get; set; }
        public string? FileName { get; set; }     // adjust to your column name
        public DateTime UploadedUtc { get; set; }
        public string? Notes { get; set; }        // adjust if you store notes
    }
}
