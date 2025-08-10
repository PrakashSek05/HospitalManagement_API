using System;
using System.Collections.Generic;

namespace HospitalManagement.API.Dtos
{
    
    public abstract class BillingBaseDto
    {
        public int BillingId { get; set; }         
        public DateTime BillingDate { get; set; }  
    }

    
    
    public class BillingItemDto
    {
        public int BillItemId { get; set; }
        public string Description { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class BillingEditDto
    {
        public int BillId { get; set; }              
        public int PatientId { get; set; }
        public int? AppointmentId { get; set; }      
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal NetAmount { get; set; }
        public bool PaidFlag { get; set; }
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }
    public class BillingListDto
    {
        public int BillingId { get; set; }
        public DateTime BillingDate { get; set; }
        public string PatientName { get; set; } = "";
        public DateTime? AppointmentDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
    }

    public class BillingDetailsDto : BillingListDto
    {
        public decimal NetAmount { get; set; }
        public bool Paid { get; set; }
        public int PatientId { get; set; }
    }

    public class BillingMiniDto
    {
        public int BillingId { get; set; }
        public DateTime BillingDate { get; set; }
        public string PatientName { get; set; } = "";
        public DateTime? AppointmentDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
    }

    public class BillingDto
    {
        public int BillingId { get; set; }
        public DateTime BillingDate { get; set; }
        public string? InvoiceNo { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal Tax { get; set; }
        public decimal NetAmount { get; set; }
        public bool Paid { get; set; }

        public int PatientId { get; set; }
        public string PatientName { get; set; } = "";
        public int? AppointmentId { get; set; }
        public DateTime? AppointmentDate { get; set; }
    }


}
