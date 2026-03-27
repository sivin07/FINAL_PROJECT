using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CLINICAL_MANAGEMENT.Models;

public partial class Department
{
    public int DeptId { get; set; }

    public string DeptName { get; set; } = null!;

    [JsonIgnore]


    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
