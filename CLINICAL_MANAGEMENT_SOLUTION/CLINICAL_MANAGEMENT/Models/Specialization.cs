using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CLINICAL_MANAGEMENT.Models;

public partial class Specialization
{
    public int SpecializationId { get; set; }

    public string Name { get; set; } = null!;
<<<<<<< HEAD
=======

>>>>>>> labtech
    [JsonIgnore]
    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
