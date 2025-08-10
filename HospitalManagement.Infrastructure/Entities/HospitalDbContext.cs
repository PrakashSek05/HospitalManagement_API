using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using HospitalManagement.Infrastructure.Entities;
using HospitalManagement.Core.DTOs;

namespace HospitalManagement.Infrastructure
{
    public partial class HospitalDbContext : DbContext
    {
        public HospitalDbContext()
        {
        }

        public HospitalDbContext(DbContextOptions<HospitalDbContext> options)
            : base(options)
        {
        }        
        public virtual DbSet<Appointment> Appointments { get; set; }
        public virtual DbSet<BillItem> BillItems { get; set; }
        public virtual DbSet<Billing> Billings { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Doctor> Doctors { get; set; }
        public virtual DbSet<Patient> Patients { get; set; }
        public virtual DbSet<PatientHistory> PatientHistories { get; set; }
        public virtual DbSet<PatientHistoryAttachment> PatientHistoryAttachments { get; set; }
        public virtual DbSet<Specialty> Specialties { get; set; }

        // Do NOT hard-code connection here; Program.cs provides it.
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // === Appointment ===
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCC2B8B762CC");
                entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Appointments_Doctors");

                entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Appointments_Patients");
            });

            // === BillItem ===
            modelBuilder.Entity<BillItem>(entity =>
            {
                entity.HasKey(e => e.BillItemId).HasName("PK__BillItem__47605AF516887653");
                entity.Property(e => e.Quantity).HasDefaultValue(1);
                entity.Property(e => e.TotalPrice).HasComputedColumnSql("([Quantity]*[UnitPrice])", true);

                entity.HasOne(d => d.Bill).WithMany(p => p.BillItems)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_BillItems_Billing");
            });

            // === Billing ===
            modelBuilder.Entity<Billing>(entity =>
            {
                entity.HasKey(e => e.BillId).HasName("PK__Billing__11F2FC6A331313D7");
                entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Appointment).WithMany(p => p.Billings)
                    .HasConstraintName("FK_Billing_Appointments");

                entity.HasOne(d => d.Patient).WithMany(p => p.Billings)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Billing_Patients");
            });

            // === Department ===
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DepartmentId).HasName("PK__Departme__B2079BED27F8CA82");
            });

            // === Doctor ===
            modelBuilder.Entity<Doctor>(entity =>
            {
                entity.HasKey(e => e.DoctorId).HasName("PK__Doctors__2DC00EBFEE4126B3");
                entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(getutcdate())");
                entity.Property(e => e.IsAvailable).HasDefaultValue(true);

                // Doctor -> Department (many-to-one)
                entity.HasOne(d => d.Department)
                    .WithMany(p => p.Doctors)
                    .HasForeignKey(d => d.DepartmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Doctors_Departments");

                // Doctor <-> Specialty (many-to-many) via existing table DoctorSpecialties
                entity.HasMany(d => d.Specialties)
                    .WithMany(s => s.Doctors)
                    .UsingEntity<Dictionary<string, object>>(
                        "DoctorSpecialties",
                        r => r.HasOne<Specialty>().WithMany()
                              .HasForeignKey("SpecialtyId")
                              .OnDelete(DeleteBehavior.NoAction)
                              .HasConstraintName("FK_DoctorSpecialties_Specialties"),
                        l => l.HasOne<Doctor>().WithMany()
                              .HasForeignKey("DoctorId")
                              .OnDelete(DeleteBehavior.NoAction)
                              .HasConstraintName("FK_DoctorSpecialties_Doctors"),
                        j =>
                        {
                            j.ToTable("DoctorSpecialties");
                            j.HasKey("DoctorId", "SpecialtyId")
                             .HasName("PK__DoctorSp__B0B681D52B373638");
                        });
            });

            // === Patient ===
            modelBuilder.Entity<Patient>(entity =>
            {
                entity.HasKey(e => e.PatientId).HasName("PK__Patients__970EC3662EFBB70A");
                entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(getutcdate())");
                entity.Property(e => e.Status).HasDefaultValue(true);
            });

            // === PatientHistory ===
            modelBuilder.Entity<PatientHistory>(entity =>
            {
                entity.HasKey(e => e.HistoryId).HasName("PK__PatientH__4D7B4ABDE959C583");
                entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Appointment).WithMany(p => p.PatientHistories)
                    .HasConstraintName("FK_PatientHistory_Appointments");

                entity.HasOne(d => d.Doctor).WithMany(p => p.PatientHistories)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PatientHistory_Doctors");

                entity.HasOne(d => d.Patient).WithMany(p => p.PatientHistories)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PatientHistory_Patients");
            });

            // === PatientHistoryAttachment ===
            modelBuilder.Entity<PatientHistoryAttachment>(entity =>
            {
                entity.HasKey(e => e.AttachmentId).HasName("PK__PatientH__442C64BED365722A");
                entity.Property(e => e.UploadedUtc).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.History).WithMany(p => p.PatientHistoryAttachments)
                    .HasConstraintName("FK_PatientHistoryAttachments_History");
            });

            // === Specialty ===
            modelBuilder.Entity<Specialty>(entity =>
            {
                entity.HasKey(e => e.SpecialtyId).HasName("PK__Specialt__D768F6A818EFDD84");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
