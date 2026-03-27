using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CLINICAL_MANAGEMENT.Models;

public partial class IssuedMedicine
{
    public int IssuedId { get; set; }

    public int? AppointmentId { get; set; }

    public int? PatientId { get; set; }

    public int? DoctorId { get; set; }

    public int? MedicineId { get; set; }

    public int? QuantityIssued { get; set; }

    public string? Dosage { get; set; }

    public DateTime? IssueDate { get; set; }

    public int? PrescriptionId { get; set; }

    public virtual Appointment? Appointment { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual Medicine? Medicine { get; set; }
    [JsonIgnore]
    public virtual ICollection<MedicineBill> MedicineBills { get; set; } = new List<MedicineBill>();

    public virtual Patient? Patient { get; set; }

    public virtual MedPrescription? Prescription { get; set; }
}
