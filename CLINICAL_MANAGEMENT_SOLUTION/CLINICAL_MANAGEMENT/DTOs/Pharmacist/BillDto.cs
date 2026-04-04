namespace CLINICAL_MANAGEMENT.DTOs.Pharmacist
{
    public class BillDto
    {

        public string PatientName { get; set; }
        public string MedicineName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Bill { get; set; }
        public DateTime Date { get; set; }
    }
}
