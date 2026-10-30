using System;
using System.Collections.Generic;

namespace Clininc_Management_System.Core.Models;

public partial class VwAdmindashboardstat
{
    public decimal? Totaldoctors { get; set; }

    public decimal? Totalpatients { get; set; }

    public decimal? Totalappointments { get; set; }

    public decimal? Scheduledappointments { get; set; }

    public decimal? Completedappointments { get; set; }

    public decimal? Cancelledappointments { get; set; }
}
