using System;
using System.Collections.Generic;

namespace Clininc_Management_System.Core.Models;

public partial class Specialty
{
    public decimal Specialtyid { get; set; }

    public string Specialtyname { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
