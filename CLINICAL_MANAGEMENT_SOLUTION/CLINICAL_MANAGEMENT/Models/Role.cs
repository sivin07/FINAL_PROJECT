<<<<<<< HEAD
﻿using Newtonsoft.Json;
=======
﻿
>>>>>>> labtech
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CLINICAL_MANAGEMENT.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;
<<<<<<< HEAD
=======

>>>>>>> labtech
    [JsonIgnore]
    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
