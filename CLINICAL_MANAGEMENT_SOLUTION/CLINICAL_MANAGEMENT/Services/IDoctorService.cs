using CLINICAL_MANAGEMENT.DTODoctor;

namespace CLINICAL_MANAGEMENT.Services
{
    public interface IDoctorService
    {// ── Dashboard ─────────────────────────────────────────────
        /// <summary>Returns today's AND tomorrow's appointments for this doctor.</summary>
        Task<IEnumerable<AppointmentDto>> GetDashboardAppointmentsAsync(int doctorId);

        // ── Consultation ──────────────────────────────────────────
        /// <summary>Loads patient + appointment data for the consultation page.</summary>
        Task<ConsultationDetailDto?> GetConsultationDetailAsync(int appointmentId, int doctorId);

        /// <summary>
        /// Orchestrates the full consultation save:
        /// 1. Validate input
        /// 2. Check stock for each medicine
        /// 3. Insert consultation record
        /// 4. Insert prescriptions + deduct stock
        /// 5. Insert lab requests
        /// 6. Update appointment status → Completed
        /// </summary>
        Task<SaveConsultationResponseDto> SaveConsultationAsync(SaveConsultationRequestDto request);

        // ── Dropdowns ─────────────────────────────────────────────
        Task<IEnumerable<MedicineDropdownDto>> GetMedicineDropdownAsync();
        Task<IEnumerable<LabTestDropdownDto>> GetLabTestDropdownAsync();

        // ── Patient History ───────────────────────────────────────
        /// <summary>Fetches all past consultations with prescriptions and lab requests.</summary>
        Task<IEnumerable<PatientHistoryDto>> GetPatientHistoryAsync(int patientId, int doctorId);

        // ── Lab Results ───────────────────────────────────────────
        Task<IEnumerable<LabResultDto>> GetLabResultsForDoctorAsync(int doctorId, DateTime date);
        Task<LabResultDto?> GetLabResultDetailAsync(int labRequestId, int doctorId);
    }
}
