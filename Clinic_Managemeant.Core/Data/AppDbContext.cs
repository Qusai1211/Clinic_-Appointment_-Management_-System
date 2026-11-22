using System;
using System.Collections.Generic;
using Clinic_Managemeant.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Clinic_Managemeant.Core.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Appointmentslot> Appointmentslots { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<Doctorworkinghour> Doctorworkinghours { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Specialty> Specialties { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("Data Source=localhost:1521/XEPDB1;User Id=my_user;Password=123456789;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("MY_USER")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Adminid).HasName("SYS_C008291");

            entity.ToTable("ADMINS");

            entity.HasIndex(e => e.Adminname, "UQ_ADMIN_NAME").IsUnique();

            entity.Property(e => e.Adminid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ADMINID");
            entity.Property(e => e.Adminname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ADMINNAME");
            entity.Property(e => e.Passwordhash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("PASSWORDHASH");
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.Appointmentid).HasName("SYS_C008283");

            entity.ToTable("APPOINTMENTS");

            entity.HasIndex(e => e.Doctorid, "IDX_APPT_DOCTOR");

            entity.HasIndex(e => e.Patientid, "IDX_APPT_PATIENT");

            entity.HasIndex(e => e.Slotid, "IDX_APPT_SLOT");

            entity.HasIndex(e => e.Referencenumber, "SYS_C008284").IsUnique();

            entity.Property(e => e.Appointmentid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("APPOINTMENTID");
            entity.Property(e => e.Appointmentdate)
                .HasColumnType("DATE")
                .HasColumnName("APPOINTMENTDATE");
            entity.Property(e => e.Appointmenttime)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("APPOINTMENTTIME");
            entity.Property(e => e.Doctorid)
                .HasColumnType("NUMBER")
                .HasColumnName("DOCTORID");
            entity.Property(e => e.Patientid)
                .HasColumnType("NUMBER")
                .HasColumnName("PATIENTID");
            entity.Property(e => e.Referencenumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REFERENCENUMBER");
            entity.Property(e => e.Slotid)
                .HasColumnType("NUMBER")
                .HasColumnName("SLOTID");
            entity.Property(e => e.Status)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValueSql("'Scheduled'")
                .HasColumnName("STATUS");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Doctorid)
                .HasConstraintName("FK_APPOINTMENT_DOCTOR");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Patientid)
                .HasConstraintName("FK_APPOINTMENT_PATIENT");

            entity.HasOne(d => d.Slot).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Slotid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_APPOINTMENT_SLOT");
        });

        modelBuilder.Entity<Appointmentslot>(entity =>
        {
            entity.HasKey(e => e.Slotid).HasName("SYS_C008272");

            entity.ToTable("APPOINTMENTSLOTS");

            entity.HasIndex(e => e.Doctorid, "IDX_SLOTS_DOCTOR");

            entity.HasIndex(e => new { e.Doctorid, e.Slotdate, e.Slottime }, "UQ_DOCTOR_SLOT").IsUnique();

            entity.Property(e => e.Slotid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("SLOTID");
            entity.Property(e => e.Doctorid)
                .HasColumnType("NUMBER")
                .HasColumnName("DOCTORID");
            entity.Property(e => e.Isavailable)
                .HasDefaultValueSql("1")
                .HasColumnType("NUMBER(1)")
                .HasColumnName("ISAVAILABLE");
            entity.Property(e => e.Slotdate)
                .HasColumnType("DATE")
                .HasColumnName("SLOTDATE");
            entity.Property(e => e.Slottime)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SLOTTIME");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointmentslots)
                .HasForeignKey(d => d.Doctorid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SLOT_DOCTOR");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("SYS_C008261");

            entity.ToTable("DOCTORS");

            entity.HasIndex(e => e.SpecialtyId, "IDX_DOCTORS_SPECIALTY");

            entity.Property(e => e.DoctorId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("DOCTORID");
            entity.Property(e => e.ConsultationFee)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("CONSULTATIONFEE");
            entity.Property(e => e.FullName)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("FULLNAME");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("1 ")
                .HasColumnType("NUMBER(1)")
                .HasColumnName("ISACTIVE");
            entity.Property(e => e.IsFeatured)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMBER(1)")
                .HasColumnName("ISFEATURED");
            entity.Property(e => e.ProfileImageUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("PROFILEIMAGEURL");
            entity.Property(e => e.Qualifications)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("QUALIFICATIONS");
            entity.Property(e => e.SpecialtyId)
                .HasColumnType("NUMBER")
                .HasColumnName("SPECIALTYID");

            entity.HasOne(d => d.Specialty).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_DOCTOR_SPECIALTY");
        });

        modelBuilder.Entity<Doctorworkinghour>(entity =>
        {
            entity.HasKey(e => e.Workinghoursid).HasName("SYS_C008267");

            entity.ToTable("DOCTORWORKINGHOURS");

            entity.HasIndex(e => e.Doctorid, "IDX_WORKHOURS_DOCTOR");

            entity.Property(e => e.Workinghoursid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("WORKINGHOURSID");
            entity.Property(e => e.Dayofweek)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("DAYOFWEEK");
            entity.Property(e => e.Doctorid)
                .HasColumnType("NUMBER")
                .HasColumnName("DOCTORID");
            entity.Property(e => e.Endtime)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("ENDTIME");
            entity.Property(e => e.Starttime)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("STARTTIME");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Doctorworkinghours)
                .HasForeignKey(d => d.Doctorid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_WORKHOURS_DOCTOR");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.Patientid).HasName("SYS_C008279");

            entity.ToTable("PATIENTS");

            entity.HasIndex(e => e.Email, "UQ_PATIENT_EMAIL").IsUnique();

            entity.HasIndex(e => e.Phonenumber, "UQ_PATIENT_PHONE").IsUnique();

            entity.Property(e => e.Patientid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("PATIENTID");
            entity.Property(e => e.Dateofbirth)
                .HasColumnType("DATE")
                .HasColumnName("DATEOFBIRTH");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Fullname)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("FULLNAME");
            entity.Property(e => e.Isactive)
                .HasDefaultValueSql("1 ")
                .HasColumnType("NUMBER(1)")
                .HasColumnName("ISACTIVE");
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PHONENUMBER");
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.Specialtyid).HasName("SYS_C008258");

            entity.ToTable("SPECIALTIES");

            entity.HasIndex(e => e.Specialtyname, "UQ_SPECIALTY_NAME").IsUnique();

            entity.Property(e => e.Specialtyid)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("SPECIALTYID");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Specialtyname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SPECIALTYNAME");
        });
        modelBuilder.HasSequence("SEQ_APPT_REF");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
