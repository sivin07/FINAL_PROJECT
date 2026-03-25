using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CLINICAL_MANAGEMENT.Models;

public partial class LabResult
{
    public int ResultId { get; set; }

    public int TestId { get; set; }

    public int PatientId { get; set; }

    public decimal? ActualValue { get; set; }

    public string? Remarks { get; set; }

    public string? DoctorReview { get; set; }

    public DateTime? Date { get; set; }

    public string? NormalRange { get; set; }

    [JsonIgnore]
    public virtual ICollection<LabTestResultBill> LabTestResultBills { get; set; } = new List<LabTestResultBill>();

    public virtual Patient Patient { get; set; } = null!;

    public virtual LabTest Test { get; set; } = null!;
}
