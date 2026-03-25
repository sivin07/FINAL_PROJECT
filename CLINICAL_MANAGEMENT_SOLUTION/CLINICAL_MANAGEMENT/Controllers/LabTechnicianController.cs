using CLINICAL_MANAGEMENT.Models;
using CLINICAL_MANAGEMENT.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CLINICAL_MANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabTechnicianController : ControllerBase
    {
        private readonly ILabTechnicianService _labService;

        // DI
        public LabTechnicianController(ILabTechnicianService labService)
        {
            _labService = labService;
        }

        #region 1. Get Pending Lab Tests

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<LabTestPrescription>>> GetPendingTests()
        {
            var tests = await _labService.GetPendingTests();

            if (tests == null || !tests.Value.Any())
                return NotFound("No pending lab tests");

            return Ok(tests.Value);
        }

        #endregion

        #region 2. Complete Lab Test (CORE API)

        [HttpPost("complete/{prescriptionId}")]
        public async Task<IActionResult> CompleteLabTest(int prescriptionId, LabResult labResult)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _labService.CompleteLabTest(prescriptionId, labResult);

            if (!result)
                return BadRequest("Lab test could not be completed");

            return Ok("Lab test completed successfully");
        }

        #endregion

        #region 3. Get All Lab Reports

        [HttpGet("reports")]
        public async Task<ActionResult<IEnumerable<LabResult>>> GetLabReports()
        {
            var reports = await _labService.GetLabReports();

            if (reports == null || !reports.Value.Any())
                return NotFound("No lab reports found");

            return Ok(reports.Value);
        }

        #endregion

        #region 4. Get Reports By Patient

        [HttpGet("reports/{patientId}")]
        public async Task<ActionResult<IEnumerable<LabResult>>> GetReportsByPatient(int patientId)
        {
            var reports = await _labService.GetReportsByPatient(patientId);

            if (reports == null || !reports.Value.Any())
                return NotFound("No reports found for this patient");

            return Ok(reports.Value);
        }

        #endregion

    }
}
