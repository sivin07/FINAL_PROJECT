using CLINICAL_MANAGEMENT.Models;

namespace CLINICAL_MANAGEMENT.Service
{
    public interface IReceptionService
    {
        Task<IEnumerable<Patient>> GetAllPatients();
        Task<Patient?> GetPatientById(int id);
        Task<IEnumerable<Patient>> SearchPatients(string term);
        Task<string> GenerateNextMmrNo();
        Task<Patient> AddPatient(Patient patient);
        Task<bool> UpdatePatient(Patient patient);
        Task<bool> DeletePatient(int id);

        Task<IEnumerable<DoctorSlot>> GetAvailableSlotsByDate(DateOnly date);
        Task<Appointment?> BookAppointment(int patientId, int slotId);
        Task<IEnumerable<Appointment>> GetAppointmentsByPatient(int patientId);
        Task<IEnumerable<object>> GetAllAppointments();
        Task<IEnumerable<object>> SearchAppointments(string? term, DateOnly? date);
        Task<object?> GenerateConsultationBill(int appointmentId);
    }
}