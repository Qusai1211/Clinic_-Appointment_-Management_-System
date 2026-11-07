using System;
using System.Collections.Generic;

namespace Clininc_Management_System.Core.Models;

public partial class VwAppointmentsbyspecialty
{
    public string Specialtyname { get; set; } = null!;

    public decimal? Appointmentcount { get; set; }
}
