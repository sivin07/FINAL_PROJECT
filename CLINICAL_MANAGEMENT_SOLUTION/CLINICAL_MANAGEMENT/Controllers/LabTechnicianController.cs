using CLINICAL_MANAGEMENT.DTOs.LabTech;
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

        #region 1. Get Pending Tests

        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<LabTestPrescription>>> GetPendingTests()
        {
            var result = await _labService.GetPendingTests();

            if (result == null || !result.Value.Any())
                return NotFound("No pending tests found");

            return Ok(result.Value);
        }

        #endregion

        #region 2. Complete Lab Test

        [HttpPost("complete/{prescriptionId}")]
        public async Task<IActionResult> CompleteLabTest(int prescriptionId, LabTechResultDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _labService.CompleteLabTest(prescriptionId, dto);

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
            try
            {
                var reports = await _labService.GetReportsByPatient(patientId);

                if (reports == null || !reports.Value.Any())
                    return NotFound("No reports found for this patient");

                return Ok(reports.Value);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

    }
}
