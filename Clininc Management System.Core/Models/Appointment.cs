using System;
using System.Collections.Generic;

namespace Clininc_Management_System.Core.Models;

public partial class Appointment
{
    public decimal Appointmentid { get; set; }

    public string? Referencenumber { get; set; }

    public DateTime Appointmentdate { get; set; }

    public string Appointmenttime { get; set; } = null!;

    public string? Status { get; set; }

    public decimal? Doctorid { get; set; }

    public decimal? Slotid { get; set; }

    public decimal? Patientid { get; set; }

    public virtual Doctor? Doctor { get; set; }

    public virtual Patient? Patient { get; set; }

    public virtual Appointmentslot? Slot { get; set; }
}
