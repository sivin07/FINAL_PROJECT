using CLINICAL_MANAGEMENT.DTOs.Pharmacist;
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


        public async Task<List<MedPrescription>> GetPendingPrescriptionsAsync()
        {
            return await _repo.GetPendingPrescriptionsAsync();
        }


        public async Task<List<MedPrescription>> GetIssuedPrescriptionsAsync()
        {
            return await _repo.GetIssuedPrescriptionsAsync();
        }

        public async Task<List<MedPrescription>> SearchPrescriptionsAsync(string query, string? status)
        {
            return await _repo.SearchPrescriptionsAsync(query, status);
        }

        public async Task<string> IssuePrescriptionAsync(int prescriptionId)
        {
            return await _repo.IssuePrescriptionAsync(prescriptionId);
        }


        public async Task<List<PrescriptionResponseDto>> GetPrescriptionDetails(int appointmentId)
        {
            return await _repo.GetPrescriptionDetails(appointmentId);
        }


        public async Task<List<BillDto>> GetBill(int patientId)
        {
            return await _repo.GetBill(patientId);
        }
    }
}
