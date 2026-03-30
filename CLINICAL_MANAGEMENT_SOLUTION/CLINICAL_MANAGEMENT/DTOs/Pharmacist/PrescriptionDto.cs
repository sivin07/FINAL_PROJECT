namespace CLINICAL_MANAGEMENT.DTOs.Pharmacist
{
    public class PrescriptionDto
    {

        public int PrescriptionId { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public string MedicineName { get; set; }
        public string Status { get; set; }
    }
}
