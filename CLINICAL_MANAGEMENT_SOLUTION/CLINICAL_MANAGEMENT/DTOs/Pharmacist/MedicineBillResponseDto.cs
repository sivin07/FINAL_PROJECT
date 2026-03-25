namespace CLINICAL_MANAGEMENT.DTOs.Pharmacist
{

    public class MedicineBillResponseDto
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public List<BillItemDto> IssuedMedicines { get; set; }
        public decimal TotalBill { get; set; }
    }

    public class BillItemDto
    {
        public int IssuedId { get; set; }
        public string MedicineName { get; set; }
        public int QuantityIssued { get; set; }
        public decimal Price { get; set; }
        public decimal Bill { get; set; }
        public string Dosage { get; set; }
    }
}
