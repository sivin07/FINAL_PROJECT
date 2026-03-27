<<<<<<< HEAD
﻿using Newtonsoft.Json;
=======
﻿
>>>>>>> labtech
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CLINICAL_MANAGEMENT.Models;

public partial class MedPrescription
{
    public int PrescriptionId { get; set; }

    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int MedicineId { get; set; }

    public int Quantity { get; set; }

    public int? Frequency { get; set; }

    public int DurationDays { get; set; }

    public DateTime PrescribedDate { get; set; }

    public string Status { get; set; } = null!;

    public string? Dosage { get; set; }

    public virtual Appointment Appointment { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<IssuedMedicine> IssuedMedicines { get; set; } = new List<IssuedMedicine>();

    public virtual Medicine Medicine { get; set; } = null!;

    public virtual Patient Patient { get; set; } = null!;
}
