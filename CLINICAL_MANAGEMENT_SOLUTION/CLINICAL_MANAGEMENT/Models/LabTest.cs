using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CLINICAL_MANAGEMENT.Models;

public partial class LabTest
{
    public int TestId { get; set; }

    public string TestName { get; set; } = null!;

    public decimal? Price { get; set; }

    public string? SampleType { get; set; }

    public string? NormalRange { get; set; }

    [JsonIgnore]
    public virtual ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();

    [JsonIgnore]
    public virtual ICollection<LabTestPrescription> LabTestPrescriptions { get; set; } = new List<LabTestPrescription>();
}
