using CLINICAL_MANAGEMENT.Models;
using CLINICAL_MANAGEMENT.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CLINICAL_MANAGEMENT.Services
{
    public class LabTechServiceImpl : ILabTechnicianService
    {
        private readonly ILabTechnicianRepository _labRepository;

        // DI
        public LabTechServiceImpl(ILabTechnicianRepository labRepository)
        {
            _labRepository = labRepository;
        }

        #region 1. Get Pending Tests

        public async Task<ActionResult<IEnumerable<LabTestPrescription>>> GetPendingTests()
        {
            return await _labRepository.GetPendingTests();
        }

        #endregion

        #region 2. Complete Lab Test

        public async Task<bool> CompleteLabTest(int prescriptionId, LabResult labResult)
        {
            // 🔥 Basic validation (important)
            if (labResult == null)
                throw new ArgumentNullException(nameof(labResult));

            if (labResult.ActualValue == null)
                throw new ArgumentException("Actual value is required");

            if (labResult.ActualValue < 0)
                throw new ArgumentException("Actual value cannot be negative");

            return await _labRepository.CompleteLabTest(prescriptionId, labResult);
        }

        #endregion

        #region 3. Get All Lab Reports

        public async Task<ActionResult<IEnumerable<LabResult>>> GetLabReports()
        {
            return await _labRepository.GetLabReports();
        }

        #endregion

        #region 4. Get Reports By Patient

        public async Task<ActionResult<IEnumerable<LabResult>>> GetReportsByPatient(int patientId)
        {
            if (patientId <= 0)
                throw new ArgumentException("Invalid patient ID");

            return await _labRepository.GetReportsByPatient(patientId);
        }

        #endregion

    }
}
