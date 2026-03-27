using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CLINICAL_MANAGEMENT.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;


    [JsonIgnore]

    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}
