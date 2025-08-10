namespace HospitalManagement.API.Dtos
{
    //basic detail
    public abstract class PatientBaseDto
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = "";
        public string Mrn { get; set; } = "";
        public string? Phone { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? UpdatedUtc { get; set; }
    }

    // master patient details
    public sealed class PatientMasterDto : PatientBaseDto
    {

        //while I click appointment link show this as popup
        public DateTime? Last_AppointmentDate { get; set; }

        //billing popup input param id and date
        public DateTime Last_BillingDate { get; set; }
        public DateTime? LatestVisitTime { get; set; }
        public string? LatestReason { get; set; }
    }

    // Patient edit popup 
    public class PatientDetailDto : PatientBaseDto
    {
        
        public DateOnly? Dob { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public DateTime Last_BillingDate { get; set; }
        public DateTime? LatestVisitTime { get; set; }

    }


    
    public sealed class PatientUpdateDto
    {
        public string FullName { get; set; } = "";
        public DateOnly? Dob { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public bool Status { get; set; }
    }
}
