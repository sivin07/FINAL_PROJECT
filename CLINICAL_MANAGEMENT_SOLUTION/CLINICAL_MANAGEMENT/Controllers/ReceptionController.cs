using CLINICAL_MANAGEMENT.Models;
using CLINICAL_MANAGEMENT.Service;
using Microsoft.AspNetCore.Mvc;

namespace CLINICAL_MANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceptionController : ControllerBase
    {
        private readonly IReceptionService _receptionService;

        public ReceptionController(IReceptionService receptionService)
        {
            _receptionService = receptionService;
        }

        [HttpGet("patients")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetAllPatients()
        {
            var patients = await _receptionService.GetAllPatients();
            return Ok(patients);
        }

        [HttpGet("patients/{id}")]
        public async Task<ActionResult<Patient>> GetPatientById(int id)
        {
            var patient = await _receptionService.GetPatientById(id);

            if (patient == null)
                return NotFound("Patient not found");

            return Ok(patient);
        }

        [HttpPost("patients")]
        public async Task<ActionResult> AddPatient([FromBody] Patient patient)
        {
            var result = await _receptionService.AddPatient(patient);

            return Ok(result);
        }

        [HttpPut("patients/{id}")]
        public async Task<ActionResult> UpdatePatient(int id, [FromBody] Patient patient)
        {
            if (id != patient.PatientId)
                return BadRequest("ID mismatch");

            var result = await _receptionService.UpdatePatient(patient);

            if (!result)
                return NotFound("Patient not found");

            return Ok("Updated successfully");
        }

        [HttpDelete("patients/{id}")]
        public async Task<ActionResult> DeletePatient(int id)
        {
            var result = await _receptionService.DeletePatient(id);

            if (!result)
                return NotFound("Patient not found");

            return Ok("Deleted successfully");
        }
    }
}