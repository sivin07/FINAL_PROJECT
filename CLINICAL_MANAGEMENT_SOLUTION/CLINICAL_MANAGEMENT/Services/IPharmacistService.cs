using CLINICAL_MANAGEMENT.DTOs.Pharmacist;
using CLINICAL_MANAGEMENT.Models;

namespace CLINICAL_MANAGEMENT.Services
{
    public interface IPharmacistService
    {

        Task<List<Medicine>> GetAllMedicinesAsync();
        Task<Medicine> GetMedicineByIdAsync(int id);
        Task<Medicine> AddMedicineAsync(Medicine medicine);
        Task<Medicine> UpdateMedicineAsync(Medicine medicine);

        //prescription section 
        Task<List<MedPrescription>> GetPendingPrescriptionsAsync();

        Task<List<MedPrescription>> GetIssuedPrescriptionsAsync();


        Task<List<MedPrescription>> SearchPrescriptionsAsync(string query, string? status);

        Task<string> IssuePrescriptionAsync(int prescriptionId);


        Task<List<PrescriptionResponseDto>> GetPrescriptionDetails(int appointmentId);

        Task<List<BillDto>> GetBillAsync(int appointmentId);
    }
}
