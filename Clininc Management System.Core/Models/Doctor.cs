using System;
using System.Collections.Generic;

namespace Clininc_Management_System.Core.Models;

public partial class Doctor
{
    public int DoctorId { get; set; }

    public string Fullname { get; set; } = null!;

    public string? Qualifications { get; set; }

    public decimal? Consultationfee { get; set; }

    public string? Profileimageurl { get; set; }

    public bool? Isfeatured { get; set; }

    public decimal? Specialtyid { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    public virtual ICollection<Appointmentslot> Appointmentslots { get; set; } = new List<Appointmentslot>();

    public virtual ICollection<Doctorworkinghour> Doctorworkinghours { get; set; } = new List<Doctorworkinghour>();

    public virtual Specialty? Specialty { get; set; }
}