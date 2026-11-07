using System;
using System.Collections.Generic;

namespace Clininc_Management_System.Core.Models;

public partial class Patient
{
    public decimal Patientid { get; set; }

    public string Fullname { get; set; } = null!;

    public string Phonenumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime Dateofbirth { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
