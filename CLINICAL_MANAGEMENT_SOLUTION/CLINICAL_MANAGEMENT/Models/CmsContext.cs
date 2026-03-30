using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CLINICAL_MANAGEMENT.Models;

public partial class CmsContext : DbContext
{
    public CmsContext()
    {
    }

    public CmsContext(DbContextOptions<CmsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Appointment> Appointments { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<DiagnosisDetail> DiagnosisDetails { get; set; }

    public virtual DbSet<Doctor> Doctors { get; set; }

    public virtual DbSet<DoctorSlot> DoctorSlots { get; set; }

    public virtual DbSet<IssuedMedicine> IssuedMedicines { get; set; }

    public virtual DbSet<LabResult> LabResults { get; set; }

    public virtual DbSet<LabTest> LabTests { get; set; }

    public virtual DbSet<LabTestPrescription> LabTestPrescriptions { get; set; }

    public virtual DbSet<LabTestResultBill> LabTestResultBills { get; set; }

    public virtual DbSet<MedPrescription> MedPrescriptions { get; set; }

    public virtual DbSet<Medicine> Medicines { get; set; }

    public virtual DbSet<MedicineBill> MedicineBills { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    public virtual DbSet<Qualification> Qualifications { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Specialization> Specializations { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

  
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(e => e.AppointmentId).HasName("PK__Appointm__8ECDFCC253680817");

            entity.ToTable("Appointment");

            entity.Property(e => e.ConsultationBill).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.SlotId).HasColumnName("slotId");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Scheduled");

            entity.HasOne(d => d.Doctor).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_Doctor");

            entity.HasOne(d => d.Patient).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Appointment_Patient");

            entity.HasOne(d => d.Slot).WithMany(p => p.Appointments)
                .HasForeignKey(d => d.SlotId)
                .HasConstraintName("fk_slot_appoint");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Category__19093A0B379E34D7");

            entity.ToTable("Category");

            entity.HasIndex(e => e.CategoryName, "UQ__Category__8517B2E0F6A26914").IsUnique();

            entity.Property(e => e.CategoryName).HasMaxLength(100);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DeptId).HasName("PK__Departme__014881AE11ADAFB0");

            entity.ToTable("Department");

            entity.HasIndex(e => e.DeptName, "UQ__Departme__5E508265BB035B24").IsUnique();

            entity.Property(e => e.DeptName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DiagnosisDetail>(entity =>
        {
            entity.HasKey(e => e.DiagnosisId).HasName("PK__Diagnosi__0C54CC73A9427530");

            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Diagnosis)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.DoctorNotes)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Symptoms)
                .HasMaxLength(500)
                .IsUnicode(false);

            entity.HasOne(d => d.Appointment).WithMany(p => p.DiagnosisDetails)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiagnosisDetails_Appointment");

            entity.HasOne(d => d.Doctor).WithMany(p => p.DiagnosisDetails)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiagnosisDetails_Doctor");

            entity.HasOne(d => d.Patient).WithMany(p => p.DiagnosisDetails)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DiagnosisDetails_Patient");
        });

        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.HasKey(e => e.DoctorId).HasName("PK__Doctor__2DC00EBFD0C6C137");

            entity.ToTable("Doctor");

            entity.HasIndex(e => e.StaffId, "UQ__Doctor__96D4AB16CB2FBF87").IsUnique();

            entity.Property(e => e.ConsultationFees).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Qualification)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Department).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.DeptId)
                .HasConstraintName("FK_Doctor_Dept");

            entity.HasOne(d => d.Specialization).WithMany(p => p.Doctors)
                .HasForeignKey(d => d.SpecializationId)
                .HasConstraintName("FK_Doctor_Specialization");

            entity.HasOne(d => d.Staff).WithOne(p => p.Doctor)
                .HasForeignKey<Doctor>(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Doctor_Staff");
        });

        modelBuilder.Entity<DoctorSlot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__DoctorSl__0A124AAFAC3DA3F2");

            entity.ToTable("DoctorSlot");

            entity.HasIndex(e => new { e.DoctorId, e.SlotDate, e.StartTime, e.EndTime }, "UQ_DoctorSlot").IsUnique();

            entity.Property(e => e.IsBooked).HasDefaultValue(false);

            entity.HasOne(d => d.Doctor).WithMany(p => p.DoctorSlots)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DoctorSlot_Doctor");

            entity.HasOne(d => d.Patient).WithMany(p => p.DoctorSlots)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK_DoctorSlot_Patient");
        });

        modelBuilder.Entity<IssuedMedicine>(entity =>
        {
            entity.HasKey(e => e.IssuedId).HasName("PK__IssuedMe__96CDAAF53D78080B");

            entity.ToTable("IssuedMedicine");

            entity.Property(e => e.Dosage)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.IssueDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Appointment).WithMany(p => p.IssuedMedicines)
                .HasForeignKey(d => d.AppointmentId)
                .HasConstraintName("FK_IssuedMedicine_Appointment");

            entity.HasOne(d => d.Doctor).WithMany(p => p.IssuedMedicines)
                .HasForeignKey(d => d.DoctorId)
                .HasConstraintName("FK_IssuedMedicine_Doctor");

            entity.HasOne(d => d.Medicine).WithMany(p => p.IssuedMedicines)
                .HasForeignKey(d => d.MedicineId)
                .HasConstraintName("FK_IssuedMedicine_Medicine");

            entity.HasOne(d => d.Patient).WithMany(p => p.IssuedMedicines)
                .HasForeignKey(d => d.PatientId)
                .HasConstraintName("FK_IssuedMedicine_Patient");

            entity.HasOne(d => d.Prescription).WithMany(p => p.IssuedMedicines)
                .HasForeignKey(d => d.PrescriptionId)
                .HasConstraintName("fk_Issue_prescr");
        });

        modelBuilder.Entity<LabResult>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PK__LabResul__9769020808F4C847");

            entity.ToTable("LabResult");

            entity.Property(e => e.ActualValue).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Date)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.DoctorReview)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.NormalRange).HasMaxLength(100);
            entity.Property(e => e.Remarks)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.Patient).WithMany(p => p.LabResults)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LabResult_Patient");

            entity.HasOne(d => d.Test).WithMany(p => p.LabResults)
                .HasForeignKey(d => d.TestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LabResult_Test");
        });

        modelBuilder.Entity<LabTest>(entity =>
        {
            entity.HasKey(e => e.TestId).HasName("PK__LabTest__8CC33160796AC671");

            entity.ToTable("LabTest");

            entity.Property(e => e.NormalRange).HasMaxLength(100);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.SampleType).HasMaxLength(50);
            entity.Property(e => e.TestName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<LabTestPrescription>(entity =>
        {
            entity.HasKey(e => e.PrescriptionId).HasName("PK__LabTestP__4013083237723960");

            entity.ToTable("LabTestPrescription");

            entity.Property(e => e.PrescribedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ResultDate).HasColumnType("datetime");
            entity.Property(e => e.ResultText).IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");
            entity.Property(e => e.TestName)
                .HasMaxLength(200)
                .IsUnicode(false);

            entity.HasOne(d => d.Appointment).WithMany(p => p.LabTestPrescriptions)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LabTestPrescription_Appointment");

            entity.HasOne(d => d.Doctor).WithMany(p => p.LabTestPrescriptions)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LabTestPrescription_Doctor");

            entity.HasOne(d => d.Patient).WithMany(p => p.LabTestPrescriptions)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LabTestPrescription_Patient");

            entity.HasOne(d => d.Test).WithMany(p => p.LabTestPrescriptions)
                .HasForeignKey(d => d.TestId)
                .HasConstraintName("FK_LabTestPrescription_LabTest");
        });

        modelBuilder.Entity<LabTestResultBill>(entity =>
        {
            entity.HasKey(e => e.LabTestBillId).HasName("PK__LabTestR__2DECF1FD15647350");

            entity.ToTable("LabTestResultBill");

            entity.Property(e => e.LabTestBill).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.Result).WithMany(p => p.LabTestResultBills)
                .HasForeignKey(d => d.ResultId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_LabResult_Bill");
        });

        modelBuilder.Entity<MedPrescription>(entity =>
        {
            entity.HasKey(e => e.PrescriptionId).HasName("PK__MedPresc__401308326DE88F60");

            entity.ToTable("MedPrescription");

            entity.Property(e => e.Dosage)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PrescribedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Appointment).WithMany(p => p.MedPrescriptions)
                .HasForeignKey(d => d.AppointmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MedPrescription_Appointment");

            entity.HasOne(d => d.Doctor).WithMany(p => p.MedPrescriptions)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MedPrescription_Doctor");

            entity.HasOne(d => d.Medicine).WithMany(p => p.MedPrescriptions)
                .HasForeignKey(d => d.MedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MedPrescription_Medicine");

            entity.HasOne(d => d.Patient).WithMany(p => p.MedPrescriptions)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MedPrescription_Patient");
        });

        modelBuilder.Entity<Medicine>(entity =>
        {
            entity.HasKey(e => e.MedicineId).HasName("PK__Medicine__4F2128902D479D1C");

            entity.ToTable("Medicine");

            entity.Property(e => e.MedicineDescription)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.MedicineName)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Category).WithMany(p => p.Medicines)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Medicine_Category");
        });

        modelBuilder.Entity<MedicineBill>(entity =>
        {
            entity.HasKey(e => e.MedicineBillId).HasName("PK__Medicine__EF3B0260CEA0B249");

            entity.ToTable("MedicineBill");

            entity.Property(e => e.Bill).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.IssuedMedicine).WithMany(p => p.MedicineBills)
                .HasForeignKey(d => d.IssuedMedicineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_medicine_bill");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("PK__Patient__970EC366CD9F54CB");

            entity.ToTable("Patient");

            entity.HasIndex(e => e.Mmrno, "UQ__Patient__561424152EDCB2D5").IsUnique();

            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(5)
                .IsUnicode(false);
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Mmrno)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("MMRNo");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Active");
        });

        modelBuilder.Entity<Qualification>(entity =>
        {
            entity.HasKey(e => e.QualificationId).HasName("PK__Qualific__C95C12AA33FB949F");

            entity.ToTable("Qualification");

            entity.Property(e => e.DegreeName).HasMaxLength(30);
            entity.Property(e => e.UniversityName).HasMaxLength(50);

            entity.HasOne(d => d.Doctor).WithMany(p => p.Qualifications)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_quali_doc");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1A3EFC67E8");

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160EB5CCFC8").IsUnique();

            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Specialization>(entity =>
        {
            entity.HasKey(e => e.SpecializationId).HasName("PK__Speciali__5809D86FC464F1C0");

            entity.ToTable("Specialization");

            entity.HasIndex(e => e.Name, "UQ__Speciali__737584F663A3F41B").IsUnique();

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staff__96D4AB17643D9804");

            entity.HasIndex(e => e.Username, "UQ__Staff__536C85E40223E524").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Staff__A9D1053482275A45").IsUnique();

            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Dob).HasColumnName("DOB");
            entity.Property(e => e.Doj).HasColumnName("DOJ");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Active");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Role).WithMany(p => p.Staff)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Staff_Role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
