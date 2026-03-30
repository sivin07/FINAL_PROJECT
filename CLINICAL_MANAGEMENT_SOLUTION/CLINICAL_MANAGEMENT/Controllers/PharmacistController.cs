using CLINICAL_MANAGEMENT.Models;
using CLINICAL_MANAGEMENT.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CLINICAL_MANAGEMENT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PharmacistController : ControllerBase
    {


        private readonly IPharmacistService _service;

        public PharmacistController(IPharmacistService service)
        {
            _service = service;
        }

        // GET: api/pharmacist/medicines
        [HttpGet("medicines")]
        public async Task<IActionResult> GetAllMedicines()
        {
            var medicines = await _service.GetAllMedicinesAsync();
            return Ok(medicines);
        }

        // GET: api/pharmacist/medicines/5
        [HttpGet("medicines/{id}")]
        public async Task<IActionResult> GetMedicineById(int id)
        {
            var medicine = await _service.GetMedicineByIdAsync(id);
            if (medicine == null)
                return NotFound("Medicine not found");
            return Ok(medicine);
        }

        // POST: api/pharmacist/medicines
        [HttpPost("medicines")]
        public async Task<IActionResult> AddMedicine([FromBody] Medicine medicine)
        {
            var result = await _service.AddMedicineAsync(medicine);
            return Ok(result);
        }

        // PUT: api/pharmacist/medicines/5
        [HttpPut("medicines/{id}")]
        public async Task<IActionResult> UpdateMedicine(int id, [FromBody] Medicine medicine)
        {
            if (id != medicine.MedicineId)
                return BadRequest("Medicine ID mismatch");

            var result = await _service.UpdateMedicineAsync(medicine);
            return Ok(result);
        }


        // GET: api/pharmacist/prescriptions/pending
        [HttpGet("prescriptions/pending")]
        public async Task<IActionResult> GetPendingPrescriptions()
        {
            var prescriptions = await _service.GetPendingPrescriptionsAsync();
            if (prescriptions == null || !prescriptions.Any())
                return NotFound("No pending prescriptions found");
            return Ok(prescriptions);
        }

        [HttpGet("prescriptions/issued")]
        public async Task<IActionResult> GetIssuedPrescriptions()
        {
            var prescriptions = await _service.GetIssuedPrescriptionsAsync();
            if (prescriptions == null || !prescriptions.Any())
                return NotFound("No issued prescriptions found");
            return Ok(prescriptions);
        }

        [HttpGet("prescriptions/search")]
        public async Task<IActionResult> SearchPrescriptions( [FromQuery] string query,[FromQuery] string? status)
        {
            if (string.IsNullOrEmpty(query))
                return BadRequest("Search query is required");

            var result = await _service.SearchPrescriptionsAsync(query, status);
            if (result == null || !result.Any())
                return NotFound("No prescriptions found");
            return Ok(result);
        }


        [HttpPost("prescriptions/issue/{id}")]
        public async Task<IActionResult> IssuePrescription(int id)
        {
            var result = await _service.IssuePrescriptionAsync(id);

            return result switch
            {
                "SUCCESS" => Ok("Medicine Issued Successfully"),
                "OUT_OF_STOCK" => BadRequest("Out of Stock"),
                "ALREADY_ISSUED" => BadRequest("Already Issued"),
                _ => StatusCode(500, "Error")
            };
        }



        [HttpGet("prescriptions/details/{appointmentId}")]
        public async Task<IActionResult> GetPrescriptionDetails(int appointmentId)
        {
            var data = await _service.GetPrescriptionDetails(appointmentId);

            if (data == null || !data.Any())
                return NotFound("No prescription details found");

            return Ok(data);
        }


        [HttpGet("bill/{patientId}")]
        public async Task<IActionResult> GetBill(int patientId)
        {
            var result = await _service.GetBill(patientId);

            if (result == null || !result.Any())
                return NotFound("No bill found");

            return Ok(result);
        }




    }
}
