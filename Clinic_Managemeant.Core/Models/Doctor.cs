using System;
using System.Collections.Generic;

namespace Clinic_Managemeant.Core.Models;

public partial class Doctor
{
    public long DoctorId { get; set; }
    public string FullName { get; set; } = null!;
    public string? Qualifications { get; set; }
    public decimal? ConsultationFee { get; set; }
    public string? ProfileImageUrl { get; set; }
    public bool? IsFeatured { get; set; }
    public long? SpecialtyId { get; set; }
    public bool? IsActive { get; set; }

    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public virtual ICollection<Appointmentslot> Appointmentslots { get; set; } = new List<Appointmentslot>();
    public virtual ICollection<Doctorworkinghour> Doctorworkinghours { get; set; } = new List<Doctorworkinghour>();
    public virtual Specialty? Specialty { get; set; }
}