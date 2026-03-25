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
    }
}
