namespace HospitalManagement.API.Dtos
{
    public class PatientMasterDto
    {
        public int PatientId { get; set; }
        public string FullName { get; set; } = "";   // adjust to your Patient name property
        public DateTime? LatestVisitTime { get; set; }
        public string? LatestReason { get; set; }     // from latest PatientHistory (adjust prop name)
        public DateTime CreatedUtc { get; set; }
        public bool Status { get; set; }
    }
}
