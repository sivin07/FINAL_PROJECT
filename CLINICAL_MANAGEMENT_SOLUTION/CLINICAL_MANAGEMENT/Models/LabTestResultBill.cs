using System;
using System.Collections.Generic;

namespace CLINICAL_MANAGEMENT.Models;

public partial class LabTestResultBill
{
    public int LabTestBillId { get; set; }

    public int ResultId { get; set; }

    public decimal LabTestBill { get; set; }

    public virtual LabResult Result { get; set; } = null!;
}
