namespace CLINICAL_MANAGEMENT.DTOs.Pharmacist
{
    public class IssueMedicineDto
    {

        public int PrescriptionId { get; set; }
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int MedicineId { get; set; }
        public int QuantityIssued { get; set; }
        public string Dosage { get; set; }
    }
}
