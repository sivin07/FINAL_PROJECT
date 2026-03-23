using System;
using System.Collections.Generic;

namespace CLINICAL_MANAGEMENT.Models;

public partial class Qualification
{
    public int QualificationId { get; set; }

    public int DoctorId { get; set; }

    public string DegreeName { get; set; } = null!;

    public string UniversityName { get; set; } = null!;

    public virtual Doctor Doctor { get; set; } = null!;
}
