using CLINICAL_MANAGEMENT.DTODoctor;

namespace CLINICAL_MANAGEMENT.Repositories
{
    public interface IDoctorRepository
    {
        // ── Appointments ──────────────────────────────────────────
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByDoctorAndDateAsync(int doctorId, DateTime date);
        Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status);

        // ── Consultation ──────────────────────────────────────────
        Task<ConsultationDetailDto?> GetConsultationDetailByAppointmentAsync(int appointmentId);
        Task<int> InsertConsultationAsync(int appointmentId, int patientId, int doctorId,
                                          string symptoms, string diagnosis, string? doctorNotes);

        // ── Prescription (Medicines) ──────────────────────────────
        Task<int> InsertPrescriptionAsync(int consultationId, int medicineId,
                                          int frequency, int duration, int quantity);

        // ── Stock (Pharmacist Module Integration) ─────────────────
        Task<int> GetMedicineStockAsync(int medicineId);
        Task<bool> DeductMedicineStockAsync(int medicineId, int quantity);
        Task<IEnumerable<MedicineDropdownDto>> GetMedicineDropdownAsync();

        // ── Lab Tests (Lab Technician Module Integration) ─────────
        Task<IEnumerable<LabTestDropdownDto>> GetLabTestDropdownAsync();
        Task<int> InsertLabRequestAsync(int consultationId, int patientId, int doctorId,
                                        int labTestId, string? specialInstructions);

        // ── Patient History ───────────────────────────────────────
        Task<IEnumerable<PatientHistoryDto>> GetPatientHistoryAsync(int patientId);
        Task<IEnumerable<PrescriptionItemDto>> GetPrescriptionsByConsultationAsync(int consultationId);
        Task<IEnumerable<LabRequestItemDto>> GetLabRequestsByConsultationAsync(int consultationId);

        // ── Lab Results ───────────────────────────────────────────
        Task<IEnumerable<LabResultDto>> GetLabResultsByDoctorAsync(int doctorId, DateTime date);
        Task<LabResultDto?> GetLabResultByRequestIdAsync(int labRequestId);
    }
}
