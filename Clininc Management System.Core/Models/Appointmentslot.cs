using System;
using System.Collections.Generic;

namespace Clininc_Management_System.Core.Models;

public partial class Appointmentslot
{
    public decimal Slotid { get; set; }

    public DateTime Slotdate { get; set; }

    public string Slottime { get; set; } = null!;

    public bool? Isavailable { get; set; }

    public decimal? Doctorid { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual Doctor? Doctor { get; set; }
}
