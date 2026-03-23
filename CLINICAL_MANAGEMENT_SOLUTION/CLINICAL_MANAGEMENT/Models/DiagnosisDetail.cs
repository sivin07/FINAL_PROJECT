using System;
using System.Collections.Generic;

namespace CLINICAL_MANAGEMENT.Models;

public partial class DiagnosisDetail
{
    public int DiagnosisId { get; set; }

    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public string? Symptoms { get; set; }

    public string? Diagnosis { get; set; }

    public string? DoctorNotes { get; set; }

    public DateTime? Date { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
