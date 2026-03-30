using CLINICAL_MANAGEMENT.Models;
using Microsoft.AspNetCore.Mvc;

namespace CLINICAL_MANAGEMENT.Services
{
    public interface ILabTechnicianService
    {
        // 1. Get pending tests
        Task<ActionResult<IEnumerable<LabTestPrescription>>> GetPendingTests();

        // 2. Complete lab test (Result + Bill + Status update)
        Task<bool> CompleteLabTest(int prescriptionId, LabResult labResult);

        // 3. Get all lab reports
        Task<ActionResult<IEnumerable<LabResult>>> GetLabReports();

        // 4. Get reports by patient
        Task<ActionResult<IEnumerable<LabResult>>> GetReportsByPatient(int patientId);
    }
}
