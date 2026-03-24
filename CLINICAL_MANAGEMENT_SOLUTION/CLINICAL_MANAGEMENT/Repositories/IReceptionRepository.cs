using CLINICAL_MANAGEMENT.Models;

namespace CLINICAL_MANAGEMENT.Repository
{
    public interface IReceptionRepository
    {
        Task<IEnumerable<Patient>> GetAllPatients();
        Task<Patient?> GetPatientById(int id);
        Task<Patient> AddPatient(Patient patient);
        Task<bool> UpdatePatient(Patient patient);
        Task<bool> DeletePatient(int id);
    }
}



       