using System;
using System.Collections.Generic;

namespace Clinic_Managemeant.Core.Models;

public partial class Patient
{
    public decimal Patientid { get; set; }

    public string Fullname { get; set; } = null!;

    public string Phonenumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime Dateofbirth { get; set; }

    public bool? Isactive { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
