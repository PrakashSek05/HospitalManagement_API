namespace HospitalManagement.Core.DTOs;

public class DashboardSummaryDto
{
    public int ActiveAppointments { get; set; }
    public int ActivePatients { get; set; }
    public int TotalPatients { get; set; }
    public int PendingBills { get; set; }
    public int CompletedBills { get; set; }
    public int AvailableDoctors { get; set; }
    public int TotalDoctors { get; set; }
}
