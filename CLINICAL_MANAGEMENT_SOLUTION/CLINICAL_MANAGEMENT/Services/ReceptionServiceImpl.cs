using CLINICAL_MANAGEMENT.Models;
using CLINICAL_MANAGEMENT.Repository;

namespace CLINICAL_MANAGEMENT.Service
{
    public class ReceptionServiceImpl : IReceptionService
    {
        private readonly IReceptionRepository _receptionRepository;

        public ReceptionServiceImpl(IReceptionRepository receptionRepository)
        {
            _receptionRepository = receptionRepository;
        }

        public async Task<IEnumerable<Patient>> GetAllPatients()
        {
            return await _receptionRepository.GetAllPatients();
        }

        public async Task<Patient?> GetPatientById(int id)
        {
            return await _receptionRepository.GetPatientById(id);
        }

        public async Task<IEnumerable<Patient>> SearchPatients(string term)
        {
            return await _receptionRepository.SearchPatients(term);
        }

        public async Task<string> GenerateNextMmrNo()
        {
            return await _receptionRepository.GenerateNextMmrNo();
        }

        public async Task<Patient> AddPatient(Patient patient)
        {
            return await _receptionRepository.AddPatient(patient);
        }

        public async Task<bool> UpdatePatient(Patient patient)
        {
            return await _receptionRepository.UpdatePatient(patient);
        }

        public async Task<bool> DeletePatient(int id)
        {
            return await _receptionRepository.DeletePatient(id);
        }

        public async Task<IEnumerable<DoctorSlot>> GetAvailableSlotsByDate(DateOnly date)
        {
            return await _receptionRepository.GetAvailableSlotsByDate(date);
        }

        public async Task<Appointment?> BookAppointment(int patientId, int slotId)
        {
            return await _receptionRepository.BookAppointment(patientId, slotId);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatient(int patientId)
        {
            return await _receptionRepository.GetAppointmentsByPatient(patientId);
        }

        public async Task<IEnumerable<object>> GetAllAppointments()
        {
            return await _receptionRepository.GetAllAppointments();
        }

        public async Task<IEnumerable<object>> SearchAppointments(string? term, DateOnly? date)
        {
            return await _receptionRepository.SearchAppointments(term, date);
        }

        public async Task<object?> GenerateConsultationBill(int appointmentId)
        {
            return await _receptionRepository.GenerateConsultationBill(appointmentId);
        }
    }
}