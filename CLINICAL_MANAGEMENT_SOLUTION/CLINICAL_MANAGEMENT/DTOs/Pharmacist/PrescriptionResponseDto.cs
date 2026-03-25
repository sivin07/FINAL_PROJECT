namespace CLINICAL_MANAGEMENT.DTOs.Pharmacist
{
    public class PrescriptionResponseDto
    {

        public int PrescriptionId { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public string Frequency { get; set; }
        public int DurationDays { get; set; }
        public string Dosage { get; set; }
        public string Status { get; set; }
        public int MedicineStock { get; set; }
        public DateTime PrescribedDate { get; set; }
    }
}
