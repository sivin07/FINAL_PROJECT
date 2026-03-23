using System;
using System.Collections.Generic;

namespace CLINICAL_MANAGEMENT.Models;

public partial class Appointment
{
    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int? TokenNumber { get; set; }

    public DateOnly AppointmentDate { get; set; }

    public string? Status { get; set; }

    public decimal? ConsultationBill { get; set; }

    public int? SlotId { get; set; }

    public virtual ICollection<DiagnosisDetail> DiagnosisDetails { get; set; } = new List<DiagnosisDetail>();

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual ICollection<IssuedMedicine> IssuedMedicines { get; set; } = new List<IssuedMedicine>();

    public virtual ICollection<LabTestPrescription> LabTestPrescriptions { get; set; } = new List<LabTestPrescription>();

    public virtual ICollection<MedPrescription> MedPrescriptions { get; set; } = new List<MedPrescription>();

    public virtual Patient Patient { get; set; } = null!;

    public virtual DoctorSlot? Slot { get; set; }
}
