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
    }
}
