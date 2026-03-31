
using CLINICAL_MANAGEMENT.Models;
using Microsoft.AspNetCore.Mvc;

namespace CLINICAL_MANAGEMENT.Repositories
{
    public interface ILabTechnicianRepository
    {

        // Get all pending lab prescriptions
        Task<ActionResult<IEnumerable<LabTestPrescription>>> GetPendingTests();

        // Complete lab test (Result + Bill + Status)
        Task<bool> CompleteLabTest(int prescriptionId, LabResult labResult);

        // Get all lab reports
        Task<ActionResult<IEnumerable<LabResult>>> GetLabReports();

        // Get reports by patient
        Task<ActionResult<IEnumerable<LabResult>>> GetReportsByPatient(int patientId);
    }
}
