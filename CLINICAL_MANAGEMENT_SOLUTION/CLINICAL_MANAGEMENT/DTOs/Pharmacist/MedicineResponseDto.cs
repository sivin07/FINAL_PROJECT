namespace CLINICAL_MANAGEMENT.DTOs.Pharmacist
{
    public class MedicineResponseDto
    {

        public int MedicineId { get; set; }
        public string MedicineName { get; set; }
        public string MedicineDescription { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateOnly ExpiryDate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
