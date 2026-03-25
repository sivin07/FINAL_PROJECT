using CLINICAL_MANAGEMENT.Models;

namespace CLINICAL_MANAGEMENT.Services
{
    public interface IPharmacistService
    {

        Task<List<Medicine>> GetAllMedicinesAsync();
        Task<Medicine> GetMedicineByIdAsync(int id);
        Task<Medicine> AddMedicineAsync(Medicine medicine);
        Task<Medicine> UpdateMedicineAsync(Medicine medicine);
    }
}
