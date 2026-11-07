using System;
using System.Collections.Generic;

namespace Clininc_Management_System.Core.Models;

public partial class Admin
{
    public decimal Adminid { get; set; }

    public string Adminname { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;
}
