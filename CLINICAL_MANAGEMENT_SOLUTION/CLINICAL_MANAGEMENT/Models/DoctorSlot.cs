
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CLINICAL_MANAGEMENT.Models;

public partial class DoctorSlot
{
    public int SlotId { get; set; }

    public int DoctorId { get; set; }

    public DateOnly SlotDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public bool? IsBooked { get; set; }

    public int? PatientId { get; set; }

    [JsonIgnore]

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Doctor Doctor { get; set; } = null!;

    public virtual Patient? Patient { get; set; }
}
