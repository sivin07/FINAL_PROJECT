using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CLINICAL_MANAGEMENT.Models;

public partial class Medicine
{
    public int MedicineId { get; set; }

    public string MedicineName { get; set; } = null!;

    public string MedicineDescription { get; set; } = null!;

    public int? Quantity { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public decimal? Price { get; set; }

    public int? CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    [JsonIgnore]
    public virtual ICollection<IssuedMedicine> IssuedMedicines { get; set; } = new List<IssuedMedicine>();

    [JsonIgnore]
    public virtual ICollection<MedPrescription> MedPrescriptions { get; set; } = new List<MedPrescription>();
}
