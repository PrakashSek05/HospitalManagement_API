namespace HospitalManagement.API.Dtos
{    
    public abstract class AppointmentBaseDto
    {
        public string? DoctorName { get; set; }
        public DateTime? VisitDateTime { get; set; }
        public string? Status { get; set; }
        public string? Reason { get; set; }
    }
    public sealed class AppointmentEditDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime VisitDateTime { get; set; }
        public string Status { get; set; } = "Scheduled";
        public string? Reason { get; set; }
    }
    public sealed class AppointmentViewDto : AppointmentBaseDto
    {

    }

    // show Apointment details in master page
    public sealed class AppointmentMasterDto : AppointmentBaseDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = "";
        public int DoctorId { get; set; }        
        public DateTime? LastVisitDateTime { get; set; }
        public string? LastDoctorName { get; set; }
        public bool? IsEdit { get; set; }
    }
}
