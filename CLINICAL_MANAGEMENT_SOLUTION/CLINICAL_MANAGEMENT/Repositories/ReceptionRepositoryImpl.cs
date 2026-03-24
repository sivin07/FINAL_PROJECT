using CLINICAL_MANAGEMENT.Models;
using Microsoft.EntityFrameworkCore;

namespace CLINICAL_MANAGEMENT.Repository
{
    public class ReceptionRepositoryImpl : IReceptionRepository
    {
        private readonly CmsContext _context;

        public ReceptionRepositoryImpl(CmsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Patient>> GetAllPatients()
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task<Patient?> GetPatientById(int id)
        {
            return await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == id);
        }

        public async Task<Patient> AddPatient(Patient patient)
        {
            await _context.Patients.AddAsync(patient);
            await _context.SaveChangesAsync();
            return patient;
        }

        public async Task<bool> UpdatePatient(Patient patient)
        {
            var existingPatient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == patient.PatientId);

            if (existingPatient == null)
                return false;

            existingPatient.Mmrno = patient.Mmrno;
            existingPatient.Name = patient.Name;
            existingPatient.Address = patient.Address;
            existingPatient.Gender = patient.Gender;
            existingPatient.Dob = patient.Dob;
            existingPatient.BloodGroup = patient.BloodGroup;
            existingPatient.Phone = patient.Phone;
            existingPatient.Status = patient.Status;
            existingPatient.Email = patient.Email;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePatient(int id)
        {
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
                return false;

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}