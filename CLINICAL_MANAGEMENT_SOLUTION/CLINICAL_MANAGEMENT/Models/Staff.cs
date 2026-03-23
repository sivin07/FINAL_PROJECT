using System;
using System.Collections.Generic;

namespace CLINICAL_MANAGEMENT.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public DateOnly Doj { get; set; }

    public string? Address { get; set; }

    public string? Gender { get; set; }

    public string? Phone { get; set; }

    public int RoleId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Status { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual Role Role { get; set; } = null!;
}
