using System;
using System.Collections.Generic;

namespace CLINICAL_MANAGEMENT.Models;

public partial class LabTestPrescription
{
    public int PrescriptionId { get; set; }

    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int? TestId { get; set; }

    public string TestName { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime PrescribedDate { get; set; }

    public string Status { get; set; } = null!;

    public string? ResultText { get; set; }

    public DateTime? ResultDate { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;

    public virtual LabTest? Test { get; set; }
}
