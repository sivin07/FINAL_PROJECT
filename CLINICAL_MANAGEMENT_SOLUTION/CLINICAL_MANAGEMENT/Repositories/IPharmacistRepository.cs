using CLINICAL_MANAGEMENT.DTOs.Pharmacist;
using CLINICAL_MANAGEMENT.Models;

namespace CLINICAL_MANAGEMENT.Repositories
{
    public interface IPharmacistRepository
    {




        Task<List<Medicine>> GetAllMedicinesAsync();
        Task<Medicine> GetMedicineByIdAsync(int id);
        Task<Medicine> AddMedicineAsync(Medicine medicine);
        Task<Medicine> UpdateMedicineAsync(Medicine medicine);
        //precription section 
        //list all pending  precription 

        Task<List<MedPrescription>> GetPendingPrescriptionsAsync();

        Task<List<MedPrescription>> GetIssuedPrescriptionsAsync();


        Task<List<MedPrescription>> SearchPrescriptionsAsync(string query, string? status);


        Task<string> IssuePrescriptionAsync(int prescriptionId);


        Task<List<PrescriptionResponseDto>> GetPrescriptionDetails(int appointmentId);


        Task<List<BillDto>> GetBill(int patientId);
    }
}
