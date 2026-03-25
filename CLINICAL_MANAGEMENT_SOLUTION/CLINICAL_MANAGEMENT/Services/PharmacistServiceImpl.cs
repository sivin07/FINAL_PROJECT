using CLINICAL_MANAGEMENT.Models;
using CLINICAL_MANAGEMENT.Repositories;

namespace CLINICAL_MANAGEMENT.Services
{
    public class PharmacistServiceImpl:IPharmacistService
    {

        private readonly IPharmacistRepository _repo;

        public PharmacistServiceImpl(IPharmacistRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<Medicine>> GetAllMedicinesAsync()
        {
            return await _repo.GetAllMedicinesAsync();
        }

        public async Task<Medicine> GetMedicineByIdAsync(int id)
        {
            return await _repo.GetMedicineByIdAsync(id);
        }

        public async Task<Medicine> AddMedicineAsync(Medicine medicine)
        {
            return await _repo.AddMedicineAsync(medicine);
        }

        public async Task<Medicine> UpdateMedicineAsync(Medicine medicine)
        {
            return await _repo.UpdateMedicineAsync(medicine);
        }
    }
}
