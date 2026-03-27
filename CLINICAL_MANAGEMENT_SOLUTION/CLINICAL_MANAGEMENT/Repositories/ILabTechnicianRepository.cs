using CLINICAL_MANAGEMENT.Models;
using Microsoft.AspNetCore.Mvc;

namespace CLINICAL_MANAGEMENT.Repositories
{
    public interface ILabTechnicianRepository
    {

        // 1. Get all pending lab tests
        Task<ActionResult<IEnumerable<LabTestPrescription>>> GetPendingTests();

        // 2. Add lab result
        Task<ActionResult<LabResult>> AddLabResult(LabResult labResult);

        // 3. Update prescription status (Completed after result)
        Task<bool> UpdatePrescriptionStatus(int prescriptionId);

        // 4. Generate bill
        Task<ActionResult<LabTestResultBill>> GenerateLabBill(LabTestResultBill bill);

        // 5. View lab report (Result + Patient + Test)
        Task<ActionResult<IEnumerable<LabResult>>> GetLabReports();

        // 6. Get report by patient
        Task<ActionResult<IEnumerable<LabResult>>> GetReportsByPatient(int patientId);

        Task<bool> CompleteLabTest(int prescriptionId, LabResult labResult);
    }
}
