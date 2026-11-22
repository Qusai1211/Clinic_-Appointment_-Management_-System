using System;
using System.Collections.Generic;

namespace Clinic_Managemeant.Core.Models;

public partial class Admin
{
    public decimal Adminid { get; set; }

    public string Adminname { get; set; } = null!;

    public string Passwordhash { get; set; } = null!;
    public object? Roleid { get; set; }
    public object? Isactive { get; set; }
    public object? Email { get; set; }
}
