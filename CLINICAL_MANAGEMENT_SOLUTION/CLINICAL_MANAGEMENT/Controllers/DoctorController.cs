using CLINICAL_MANAGEMENT.DTODoctor;
using CLINICAL_MANAGEMENT.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CLINICAL_MANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _service;
        private readonly ILogger<DoctorController> _logger;

        public DoctorController(IDoctorService service, ILogger<DoctorController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // ── Helper: extract DoctorId from JWT claims ─────────────
        private int GetDoctorIdFromClaims()
        {
            var claim = User.FindFirst("DoctorId")?.Value
                        ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(claim, out int doctorId) || doctorId <= 0)
                throw new UnauthorizedAccessException("Invalid doctor identity in token.");

            return doctorId;
        }

        // ══════════════════════════════════════════════════════════
        //  1. DASHBOARD — GET APPOINTMENTS
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// GET /api/doctor/dashboard/appointments
        /// Returns today's and tomorrow's appointments for the logged-in doctor.
        /// DoctorId is read from the JWT token — not from the URL.
        /// </summary>
        [HttpGet("dashboard/appointments")]
        [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<AppointmentDto>>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetDashboardAppointments()
        {
            try
            {
                int doctorId = GetDoctorIdFromClaims();
                var appointments = await _service.GetDashboardAppointmentsAsync(doctorId);

                return Ok(ApiResponseDto<IEnumerable<AppointmentDto>>.Ok(
                    appointments, $"Loaded {appointments.Count()} appointments."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponseDto<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard appointments.");
                return StatusCode(500, ApiResponseDto<object>.Fail("An error occurred. Please try again."));
            }
        }

        // ══════════════════════════════════════════════════════════
        //  2. CONSULTATION DETAIL — LOAD CONSULTATION PAGE
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// GET /api/doctor/consultation/{appointmentId}
        /// Called when doctor clicks "Start Consultation".
        /// Returns patient details + existing consultation data (if already saved).
        /// Also marks appointment as InProgress.
        /// </summary>
        [HttpGet("consultation/{appointmentId:int}")]
        [ProducesResponseType(typeof(ApiResponseDto<ConsultationDetailDto>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetConsultationDetail(int appointmentId)
        {
            try
            {
                int doctorId = GetDoctorIdFromClaims();
                var detail = await _service.GetConsultationDetailAsync(appointmentId, doctorId);

                if (detail == null)
                    return NotFound(ApiResponseDto<object>.Fail(
                        $"Appointment {appointmentId} not found."));

                return Ok(ApiResponseDto<ConsultationDetailDto>.Ok(detail));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponseDto<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading consultation for AppointmentId={Id}", appointmentId);
                return StatusCode(500, ApiResponseDto<object>.Fail("An error occurred."));
            }
        }

        // ══════════════════════════════════════════════════════════
        //  3. SAVE CONSULTATION
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// POST /api/doctor/consultation/save
        /// Saves the full consultation including medicines and lab tests.
        /// 
        /// Business Logic (handled in Service):
        ///   - Validates all fields
        ///   - Calculates Quantity = Frequency × Duration per medicine
        ///   - Checks stock for ALL medicines before saving anything
        ///   - If any stock insufficient → returns error list, nothing saved
        ///   - Inserts consultation → prescriptions → lab requests
        ///   - Deducts stock from Pharmacist module
        ///   - Marks appointment as Completed
        /// </summary>
        [HttpPost("consultation/save")]
        [ProducesResponseType(typeof(ApiResponseDto<SaveConsultationResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponseDto<SaveConsultationResponseDto>), 400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SaveConsultation([FromBody] SaveConsultationRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponseDto<object>.Fail("Invalid request payload.",
                    ModelState.Values.SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage).ToList()));

            try
            {
                int doctorId = GetDoctorIdFromClaims();

                // Override DoctorId from token — never trust client-supplied DoctorId
                request.DoctorId = doctorId;

                var result = await _service.SaveConsultationAsync(request);

                if (!result.Success)
                    return BadRequest(new
                    {
                        result.Success,
                        result.Message,
                        result.StockErrors
                    });

                return Ok(ApiResponseDto<SaveConsultationResponseDto>.Ok(
                    result, result.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponseDto<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving consultation for AppointmentId={Id}",
                    request.AppointmentId);
                return StatusCode(500, ApiResponseDto<object>.Fail("An error occurred while saving."));
            }
        }

        // ══════════════════════════════════════════════════════════
        //  4. MEDICINE DROPDOWN (From Pharmacist Module)
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// GET /api/doctor/medicines
        /// Returns all active medicines with stock info from Pharmacist module.
        /// Doctor uses this to populate the medicine dropdown in the consultation form.
        /// </summary>
        [HttpGet("medicines")]
        [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<MedicineDropdownDto>>), 200)]
        public async Task<IActionResult> GetMedicines()
        {
            try
            {
                var medicines = await _service.GetMedicineDropdownAsync();
                return Ok(ApiResponseDto<IEnumerable<MedicineDropdownDto>>.Ok(medicines));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading medicine dropdown.");
                return StatusCode(500, ApiResponseDto<object>.Fail("An error occurred."));
            }
        }

        // ══════════════════════════════════════════════════════════
        //  5. LAB TEST DROPDOWN (From Lab Technician Module)
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// GET /api/doctor/labtests
        /// Returns all available lab tests from Lab Technician module.
        /// Doctor uses this to populate the lab test dropdown.
        /// </summary>
        [HttpGet("labtests")]
        [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<LabTestDropdownDto>>), 200)]
        public async Task<IActionResult> GetLabTests()
        {
            try
            {
                var labTests = await _service.GetLabTestDropdownAsync();
                return Ok(ApiResponseDto<IEnumerable<LabTestDropdownDto>>.Ok(labTests));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading lab test dropdown.");
                return StatusCode(500, ApiResponseDto<object>.Fail("An error occurred."));
            }
        }

        // ══════════════════════════════════════════════════════════
        //  6. PATIENT HISTORY
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// GET /api/doctor/patient/{patientId}/history
        /// Returns all past consultations for a patient:
        ///   - Symptoms, diagnosis, notes
        ///   - Prescription history (per consultation)
        ///   - Lab test history (per consultation)
        /// Doctor sees full patient history for continuity of care,
        /// not just their own consultations.
        /// </summary>
        [HttpGet("patient/{patientId:int}/history")]
        [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<PatientHistoryDto>>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPatientHistory(int patientId)
        {
            try
            {
                int doctorId = GetDoctorIdFromClaims();
                var history = await _service.GetPatientHistoryAsync(patientId, doctorId);

                return Ok(ApiResponseDto<IEnumerable<PatientHistoryDto>>.Ok(
                    history, $"Found {history.Count()} consultation(s) in history."));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponseDto<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading patient history for PatientId={Id}", patientId);
                return StatusCode(500, ApiResponseDto<object>.Fail("An error occurred."));
            }
        }

        // ══════════════════════════════════════════════════════════
        //  7. LAB RESULTS
        // ══════════════════════════════════════════════════════════

        /// <summary>
        /// GET /api/doctor/labresults?date=2024-01-15
        /// Returns lab results for all patients seen by this doctor on the given date.
        /// Shows status: Pending / Completed.
        /// Defaults to today if no date provided.
        /// </summary>
        [HttpGet("labresults")]
        [ProducesResponseType(typeof(ApiResponseDto<IEnumerable<LabResultDto>>), 200)]
        public async Task<IActionResult> GetLabResults([FromQuery] DateTime? date)
        {
            try
            {
                int doctorId = GetDoctorIdFromClaims();
                var targetDate = date ?? DateTime.Today;
                var results = await _service.GetLabResultsForDoctorAsync(doctorId, targetDate);

                return Ok(ApiResponseDto<IEnumerable<LabResultDto>>.Ok(results));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponseDto<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading lab results.");
                return StatusCode(500, ApiResponseDto<object>.Fail("An error occurred."));
            }
        }

        /// <summary>
        /// GET /api/doctor/labresults/{labRequestId}
        /// Returns full details of a single lab result including result values.
        /// </summary>
        [HttpGet("labresults/{labRequestId:int}")]
        [ProducesResponseType(typeof(ApiResponseDto<LabResultDto>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetLabResultDetail(int labRequestId)
        {
            try
            {
                int doctorId = GetDoctorIdFromClaims();
                var result = await _service.GetLabResultDetailAsync(labRequestId, doctorId);

                if (result == null)
                    return NotFound(ApiResponseDto<object>.Fail(
                        $"Lab result {labRequestId} not found."));

                return Ok(ApiResponseDto<LabResultDto>.Ok(result));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ApiResponseDto<object>.Fail(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading lab result {Id}", labRequestId);
                return StatusCode(500, ApiResponseDto<object>.Fail("An error occurred."));
            }
        }
    }
}



