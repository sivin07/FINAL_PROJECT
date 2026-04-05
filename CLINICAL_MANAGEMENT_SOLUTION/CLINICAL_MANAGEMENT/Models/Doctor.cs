
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CLINICAL_MANAGEMENT.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public int StaffId { get; set; }

    public string? Qualification { get; set; }

    public int? DeptId { get; set; }

    public int? SpecializationId { get; set; }

    public decimal? ConsultationFees { get; set; }



    [JsonIgnore]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();



    public virtual Department? Dept { get; set; }


    public virtual ICollection<DiagnosisDetail> DiagnosisDetails { get; set; } = new List<DiagnosisDetail>();

    [JsonIgnore]
    public virtual ICollection<DoctorSlot> DoctorSlots { get; set; } = new List<DoctorSlot>();

    [JsonIgnore]
    public virtual ICollection<IssuedMedicine> IssuedMedicines { get; set; } = new List<IssuedMedicine>();

    [JsonIgnore]
    public virtual ICollection<LabTestPrescription> LabTestPrescriptions { get; set; } = new List<LabTestPrescription>();

    [JsonIgnore]
    public virtual ICollection<MedPrescription> MedPrescriptions { get; set; } = new List<MedPrescription>();

    [JsonIgnore]
    public virtual ICollection<Qualification> Qualifications { get; set; } = new List<Qualification>();


    //public virtual Department? Department { get; set; }

    public virtual Specialization? Specialization { get; set; }

    public virtual Staff Staff { get; set; } = null!;
}
