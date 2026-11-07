using System;
using System.Collections.Generic;
using Clininc_Management_System.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Clininc_Management_System.Core.Data;

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

    public virtual DbSet<VwAdmindashboardstat> VwAdmindashboardstats { get; set; }

    public virtual DbSet<VwAppointmentsbyspecialty> VwAppointmentsbyspecialties { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("Data Source=localhost:1521/XEPDB1;User Id=my_user1;Password=123456789;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("MY_USER1")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Adminid).HasName("SYS_C008255");

            entity.ToTable("ADMINS");

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
            entity.HasKey(e => e.Appointmentid).HasName("SYS_C008247");

            entity.ToTable("APPOINTMENTS");

            entity.HasIndex(e => e.Referencenumber, "SYS_C008248").IsUnique();

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
                .HasDefaultValueSql("'Pending'")
                .HasColumnName("STATUS");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Doctorid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_APPOINTMENT_DOCTOR");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Patientid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_APPOINTMENT_PATIENT");

            entity.HasOne(d => d.Slot).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.Slotid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_APPOINTMENT_SLOT");
        });

        modelBuilder.Entity<Appointmentslot>(entity =>
        {
            entity.HasKey(e => e.Slotid).HasName("SYS_C008236");

            entity.ToTable("APPOINTMENTSLOTS");

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
            entity.HasKey(e => e.DoctorId).HasName("SYS_C008225");

            entity.ToTable("DOCTORS");

            entity.Property(e => e.DoctorId)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("DOCTORID");
            entity.Property(e => e.Consultationfee)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("CONSULTATIONFEE");
            entity.Property(e => e.Fullname)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("FULLNAME");
            entity.Property(e => e.Isfeatured)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMBER(1)")
                .HasColumnName("ISFEATURED");
            entity.Property(e => e.Profileimageurl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("PROFILEIMAGEURL");
            entity.Property(e => e.Qualifications)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("QUALIFICATIONS");
            entity.Property(e => e.Specialtyid)
                .HasColumnType("NUMBER")
                .HasColumnName("SPECIALTYID");

            entity.HasOne(d => d.Specialty).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.Specialtyid)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_DOCTOR_SPECIALTY");
        });

        modelBuilder.Entity<Doctorworkinghour>(entity =>
        {
            entity.HasKey(e => e.Workinghoursid).HasName("SYS_C008231");

            entity.ToTable("DOCTORWORKINGHOURS");

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
            entity.HasKey(e => e.Patientid).HasName("SYS_C008243");

            entity.ToTable("PATIENTS");

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
            entity.Property(e => e.Phonenumber)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("PHONENUMBER");
        });

        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.HasKey(e => e.Specialtyid).HasName("SYS_C008222");

            entity.ToTable("SPECIALTIES");

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

        modelBuilder.Entity<VwAdmindashboardstat>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_ADMINDASHBOARDSTATS");

            entity.Property(e => e.Cancelledappointments)
                .HasColumnType("NUMBER")
                .HasColumnName("CANCELLEDAPPOINTMENTS");
            entity.Property(e => e.Completedappointments)
                .HasColumnType("NUMBER")
                .HasColumnName("COMPLETEDAPPOINTMENTS");
            entity.Property(e => e.Scheduledappointments)
                .HasColumnType("NUMBER")
                .HasColumnName("SCHEDULEDAPPOINTMENTS");
            entity.Property(e => e.Totalappointments)
                .HasColumnType("NUMBER")
                .HasColumnName("TOTALAPPOINTMENTS");
            entity.Property(e => e.Totaldoctors)
                .HasColumnType("NUMBER")
                .HasColumnName("TOTALDOCTORS");
            entity.Property(e => e.Totalpatients)
                .HasColumnType("NUMBER")
                .HasColumnName("TOTALPATIENTS");
        });

        modelBuilder.Entity<VwAppointmentsbyspecialty>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("VW_APPOINTMENTSBYSPECIALTY");

            entity.Property(e => e.Appointmentcount)
                .HasColumnType("NUMBER")
                .HasColumnName("APPOINTMENTCOUNT");
            entity.Property(e => e.Specialtyname)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("SPECIALTYNAME");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
