
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace CLINICAL_MANAGEMENT.Models;

public partial class Patient
{
    public int PatientId { get; set; }

    public string Mmrno { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? Gender { get; set; }

    public DateOnly? Dob { get; set; }

    public string? BloodGroup { get; set; }

    public string? Phone { get; set; }

    public string? Status { get; set; }

    public string? Email { get; set; }

    [JsonIgnore]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [JsonIgnore]
    public virtual ICollection<DiagnosisDetail> DiagnosisDetails { get; set; } = new List<DiagnosisDetail>();

    [JsonIgnore]
    public virtual ICollection<DoctorSlot> DoctorSlots { get; set; } = new List<DoctorSlot>();

    [JsonIgnore]
    public virtual ICollection<IssuedMedicine> IssuedMedicines { get; set; } = new List<IssuedMedicine>();

    [JsonIgnore]
    public virtual ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();

    [JsonIgnore]
    public virtual ICollection<LabTestPrescription> LabTestPrescriptions { get; set; } = new List<LabTestPrescription>();

    [JsonIgnore]
    public virtual ICollection<MedPrescription> MedPrescriptions { get; set; } = new List<MedPrescription>();
}
