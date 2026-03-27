using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CLINICAL_MANAGEMENT.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

<<<<<<< HEAD

    [JsonIgnore]

=======
    [JsonIgnore]
>>>>>>> labtech
    public virtual ICollection<Medicine> Medicines { get; set; } = new List<Medicine>();
}
