using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CLINICAL_MANAGEMENT.Models;

public partial class MedicineBill
{
    public int MedicineBillId { get; set; }

    public int IssuedMedicineId { get; set; }

    public decimal Bill { get; set; }
    [JsonIgnore]
    public virtual IssuedMedicine IssuedMedicine { get; set; } = null!;
}
