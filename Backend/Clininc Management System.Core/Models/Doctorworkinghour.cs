using System;
using System.Collections.Generic;

namespace Clininc_Management_System.Core.Models;

public partial class Doctorworkinghour
{
    public decimal Workinghoursid { get; set; }

    public string Dayofweek { get; set; } = null!;

    public string Starttime { get; set; } = null!;

    public string Endtime { get; set; } = null!;

    public decimal? Doctorid { get; set; }

    public virtual Doctor? Doctor { get; set; }
}
