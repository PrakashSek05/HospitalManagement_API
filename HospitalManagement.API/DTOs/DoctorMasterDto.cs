namespace HospitalManagement.API.Dtos
{
    public class DoctorMasterDto
    {
        public int DoctorId { get; set; }
        public string FullName { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public List<string> Specialties { get; set; } = new();
        public bool IsAvailable { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime? UpdatedUtc { get; set; }
    }
}
