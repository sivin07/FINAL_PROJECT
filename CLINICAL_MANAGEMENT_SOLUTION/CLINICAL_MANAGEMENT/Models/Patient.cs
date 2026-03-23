using System;
using System.Collections.Generic;

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

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<DiagnosisDetail> DiagnosisDetails { get; set; } = new List<DiagnosisDetail>();

    public virtual ICollection<DoctorSlot> DoctorSlots { get; set; } = new List<DoctorSlot>();

    public virtual ICollection<IssuedMedicine> IssuedMedicines { get; set; } = new List<IssuedMedicine>();

    public virtual ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();

    public virtual ICollection<LabTestPrescription> LabTestPrescriptions { get; set; } = new List<LabTestPrescription>();

    public virtual ICollection<MedPrescription> MedPrescriptions { get; set; } = new List<MedPrescription>();
}
