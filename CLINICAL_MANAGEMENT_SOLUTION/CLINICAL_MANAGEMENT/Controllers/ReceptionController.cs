using CLINICAL_MANAGEMENT.Models;
using CLINICAL_MANAGEMENT.Service;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

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
                return NotFound(new { message = "Patient not found" });

            return Ok(patient);
        }

        [HttpGet("patients/search")]
        public async Task<ActionResult<IEnumerable<Patient>>> SearchPatients([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                var allPatients = await _receptionService.GetAllPatients();
                return Ok(allPatients);
            }

            var patients = await _receptionService.SearchPatients(term);
            return Ok(patients);
        }

        [HttpGet("patients/next-mmrno")]
        public async Task<ActionResult> GenerateNextMmrNo()
        {
            var mmrNo = await _receptionService.GenerateNextMmrNo();
            return Ok(new { mmrNo });
        }

        [HttpPost("patients")]
        public async Task<ActionResult> AddPatient([FromBody] Patient patient)
        {
            var validationError = ValidatePatient(patient);
            if (validationError != null)
                return BadRequest(new { message = validationError });

            var result = await _receptionService.AddPatient(patient);

            return Ok(new
            {
                message = "Patient added successfully",
                data = result
            });
        }

        [HttpPut("patients/{id}")]
        public async Task<ActionResult> UpdatePatient(int id, [FromBody] Patient patient)
        {
            if (patient == null)
                return BadRequest(new { message = "Invalid patient data" });

            if (id != patient.PatientId)
                return BadRequest(new { message = "Patient ID cannot be changed" });

            var existingPatient = await _receptionService.GetPatientById(id);

            if (existingPatient == null)
                return NotFound(new { message = "Patient not found" });

            patient.Mmrno = existingPatient.Mmrno;

            var validationError = ValidatePatient(patient);
            if (validationError != null)
                return BadRequest(new { message = validationError });

            var result = await _receptionService.UpdatePatient(patient);

            if (!result)
                return NotFound(new { message = "Patient not found" });

            return Ok(new { message = "Updated successfully" });
        }

        [HttpDelete("patients/{id}")]
        public async Task<ActionResult> DeletePatient(int id)
        {
            var result = await _receptionService.DeletePatient(id);

            if (!result)
                return NotFound(new { message = "Patient not found" });

            return Ok(new { message = "Patient marked as inactive successfully" });
        }

        [HttpGet("slots")]
        public async Task<ActionResult> GetAvailableSlotsByDate([FromQuery] DateOnly date)
        {
            var slots = await _receptionService.GetAvailableSlotsByDate(date);

            var result = slots.Select(s => new
            {
                s.SlotId,
                s.DoctorId,
                s.SlotDate,
                s.StartTime,
                s.EndTime,
                s.IsBooked
            });

            return Ok(result);
        }

        [HttpPost("appointments")]
        public async Task<ActionResult> BookAppointment([FromBody] BookAppointmentRequest request)
        {
            var appointment = await _receptionService.BookAppointment(request.PatientId, request.SlotId);

            if (appointment == null)
                return BadRequest(new { message = "Unable to book appointment" });

            return Ok(new
            {
                message = "Appointment booked successfully",
                data = appointment
            });
        }

        [HttpGet("appointments/patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByPatient(int patientId)
        {
            var appointments = await _receptionService.GetAppointmentsByPatient(patientId);
            return Ok(appointments);
        }

        [HttpGet("appointments")]
        public async Task<ActionResult> GetAllAppointments()
        {
            var appointments = await _receptionService.GetAllAppointments();
            return Ok(appointments);
        }

        [HttpGet("appointments/search")]
        public async Task<ActionResult> SearchAppointments([FromQuery] string? term, [FromQuery] DateOnly? date)
        {
            var appointments = await _receptionService.SearchAppointments(term, date);
            return Ok(appointments);
        }

        [HttpGet("billing/{appointmentId}")]
        public async Task<ActionResult> GetConsultationBill(int appointmentId)
        {
            var bill = await _receptionService.GenerateConsultationBill(appointmentId);

            if (bill == null)
                return NotFound(new { message = "Bill not found" });

            return Ok(bill);
        }

        private string? ValidatePatient(Patient patient)
        {
            if (patient == null)
                return "Invalid patient data";

            if (string.IsNullOrWhiteSpace(patient.Name) || !Regex.IsMatch(patient.Name, @"^[A-Za-z ]+$"))
                return "Name should contain only letters and spaces.";

            if (!patient.Dob.HasValue)
                return "Date of Birth is required.";

            var dob = patient.Dob.Value;
            var today = DateOnly.FromDateTime(DateTime.Today);

            int age = today.Year - dob.Year;
            if (dob > today.AddYears(-age))
                age--;

            if (age < 1 || age > 120)
                return "Age must be between 1 and 120 years.";

            if (string.IsNullOrWhiteSpace(patient.Phone) || !Regex.IsMatch(patient.Phone, @"^[6-9][0-9]{9}$"))
                return "Phone number must be 10 digits and start with 6-9.";

            if (Regex.IsMatch(patient.Phone, @"^(\d)\1{9}$"))
                return "Phone number cannot contain repeating same digits like 0000000000.";

            if (string.IsNullOrWhiteSpace(patient.Email) || !Regex.IsMatch(patient.Email, @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[A-Za-z]{2,}$"))
                return "Invalid email format.";

            if (!string.IsNullOrWhiteSpace(patient.BloodGroup) &&
                !Regex.IsMatch(patient.BloodGroup, @"^(A|B|AB|O)[+-]$"))
                return "Invalid blood group format.";

            return null;
        }
    }

    public class BookAppointmentRequest
    {
        public int PatientId { get; set; }
        public int SlotId { get; set; }
    }
}