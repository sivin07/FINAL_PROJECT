using CLINICAL_MANAGEMENT.DTOs.Pharmacist;
using CLINICAL_MANAGEMENT.Models;
using Microsoft.EntityFrameworkCore;

namespace CLINICAL_MANAGEMENT.Repositories
{
    public class PharmacistRepoImpl:IPharmacistRepository
    {
        private readonly CmsContext _context;

        public PharmacistRepoImpl(CmsContext context)
        {
            _context = context;
        }

        public async Task<List<Medicine>> GetAllMedicinesAsync()
        {
            return await _context.Medicines.ToListAsync();
        }

        public async Task<Medicine> GetMedicineByIdAsync(int id)
        {
            return await _context.Medicines
                .FirstOrDefaultAsync(m => m.MedicineId == id);
        }

        public async Task<Medicine> AddMedicineAsync(Medicine medicine)
        {
            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();
            return medicine;
        }

        public async Task<Medicine> UpdateMedicineAsync(Medicine medicine)
        {
            _context.Medicines.Update(medicine);
            await _context.SaveChangesAsync();
            return medicine;
        }


        public async Task<List<MedPrescription>> GetPendingPrescriptionsAsync()
        {
            return await _context.MedPrescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.Staff)
                .Include(p => p.Medicine)
                .Where(p => p.Status == "Prescribed")
                .ToListAsync();
        }


        public async Task<List<MedPrescription>> GetIssuedPrescriptionsAsync()
        {
            return await _context.MedPrescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.Staff)
                .Include(p => p.Medicine)
                .Where(p => p.Status == "Issued")
                .ToListAsync();
        }


        public async Task<List<MedPrescription>> SearchPrescriptionsAsync(string query, string? status)
        {
            var result = _context.MedPrescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.Staff)
                .Include(p => p.Medicine)
                .AsQueryable();

            // Filter by status if provided
            if (!string.IsNullOrEmpty(status))
                result = result.Where(p => p.Status == status);

            // Search by query
            result = result.Where(p =>
                p.Patient.Name.Contains(query) ||
                p.Patient.Mmrno.Contains(query) ||
                p.PatientId.ToString() == query ||
                p.AppointmentId.ToString() == query ||
                p.PrescriptionId.ToString() == query);

            return await result.ToListAsync();
        }



        public async Task<string> IssuePrescriptionAsync(int prescriptionId)
        {
            var result = _context.SpResultDtos
                .FromSqlRaw("EXEC sp_IssuePrescription @PrescriptionId = {0}", prescriptionId)
                .AsEnumerable()
                .FirstOrDefault();

            return result?.Result;
        }

        public async Task<List<PrescriptionResponseDto>> GetPrescriptionDetails(int appointmentId)
        {
            return await _context.MedPrescriptions
                .Where(p => p.AppointmentId == appointmentId && p.Status == "Prescribed")
                .Select(p => new PrescriptionResponseDto
                {
                    PrescriptionId = p.PrescriptionId,
                    MedicineName = p.Medicine.MedicineName,
                    Dosage = p.Dosage,
                    Quantity = p.Quantity,
                    Frequency = p.Frequency,
                    DurationDays = p.DurationDays,
                    Stock = p.Medicine.Quantity ?? 0,  // ← add this!
                    Status = p.Status                   // ← add this!
                })
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<List<BillDto>> GetBill(int appointmentId)
        {
            return await _context.MedicineBills
                .Where(b => b.IssuedMedicine.AppointmentId == appointmentId)
                .Select(b => new BillDto
                {
                    PatientName = b.IssuedMedicine.Patient.Name,
                    MedicineName = b.IssuedMedicine.Medicine.MedicineName,
                    Quantity = b.IssuedMedicine.QuantityIssued ?? 0,
                    Price = b.IssuedMedicine.Medicine.Price ?? 0,
                    Bill = b.Bill,
                    Date = b.IssuedMedicine.IssueDate ?? DateTime.Now
                })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
