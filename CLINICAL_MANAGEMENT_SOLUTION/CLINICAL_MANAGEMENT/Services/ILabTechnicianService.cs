using CLINICAL_MANAGEMENT.Models;
using Microsoft.AspNetCore.Mvc;

namespace CLINICAL_MANAGEMENT.Services
{
    public interface ILabTechnicianService
    {
        // Get all pending tests
        Task<ActionResult<IEnumerable<LabTestPrescription>>> GetPendingTests();

        // Complete lab test (main workflow)
        Task<bool> CompleteLabTest(int prescriptionId, LabResult labResult);

        // Get all lab reports
        Task<ActionResult<IEnumerable<LabResult>>> GetLabReports();

        // Get reports by patient
        Task<ActionResult<IEnumerable<LabResult>>> GetReportsByPatient(int patientId);
    }
}
