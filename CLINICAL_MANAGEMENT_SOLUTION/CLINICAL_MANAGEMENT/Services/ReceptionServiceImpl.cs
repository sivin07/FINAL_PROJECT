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
    }
}