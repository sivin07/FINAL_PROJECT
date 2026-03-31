namespace CLINICAL_MANAGEMENT.DTOs.Pharmacist
{
    public class PrescriptionResponseDto
    {

        public int PrescriptionId { get; set; }
        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public int Quantity { get; set; }
        public int? Frequency { get; set; }
        public int DurationDays { get; set; }
        public int Stock { get; set; }
        public string? Status { get; set; }

    }
}