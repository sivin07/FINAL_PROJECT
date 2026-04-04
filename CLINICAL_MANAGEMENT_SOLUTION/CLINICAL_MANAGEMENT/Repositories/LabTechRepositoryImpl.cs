using CLINICAL_MANAGEMENT.DTOs.LabTech;
using CLINICAL_MANAGEMENT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CLINICAL_MANAGEMENT.Repositories
{
    public class LabTechRepositoryImpl : ILabTechnicianRepository
    {
        
        private readonly CmsContext _context;

        // DI
        public LabTechRepositoryImpl(CmsContext context)
        {
            _context = context;
        }

        #region 1. Get Pending Tests

        public async Task<ActionResult<IEnumerable<LabTestPrescription>>> GetPendingTests()
        {
            return await _context.LabTestPrescriptions
                .Include(p => p.Patient)
                .Include(p => p.Doctor)
                    .ThenInclude(d => d.Staff)
                .Include(p => p.Test)
                .Where(p => p.Status == "Pending")
                .ToListAsync();
        }

        #endregion

        #region 2. Complete Lab Test (CORE LOGIC)

        public async Task<bool> CompleteLabTest(int prescriptionId, LabTechResultDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // STEP 1: Get Prescription with Test
                var prescription = await _context.LabTestPrescriptions
                    .Include(p => p.Test)
                    .FirstOrDefaultAsync(p => p.PrescriptionId == prescriptionId);

                if (prescription == null)
                    throw new Exception("Prescription not found");

                if (prescription.Status == "Completed")
                    throw new Exception("Test already completed");

                if (prescription.TestId == null)
                    throw new Exception("Invalid Test");

                // STEP 2: Create LabResult (from DTO + system values)
                var labResult = new LabResult
                {
                    ActualValue = dto.ActualValue,
                    Remarks = dto.Remarks,
                    DoctorReview = dto.DoctorReview,

                    PatientId = prescription.PatientId,
                    TestId = prescription.TestId.Value,
                    Date = DateTime.Now,

                    // 🔥 from LabTest
                    NormalRange = prescription.Test?.NormalRange
                };

                await _context.LabResults.AddAsync(labResult);
                await _context.SaveChangesAsync();

                // STEP 3: Generate Bill automatically
                var bill = new LabTestResultBill
                {
                    ResultId = labResult.ResultId,
                    LabTestBill = prescription.Test?.Price ?? 0
                };

                await _context.LabTestResultBills.AddAsync(bill);
                await _context.SaveChangesAsync();

                // STEP 4: Update Prescription Status
                prescription.Status = "Completed";
                prescription.ResultDate = DateTime.Now;

                await _context.SaveChangesAsync();

                // STEP 5: Commit
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        #endregion

        #region 3. Get All Lab Reports

        public async Task<ActionResult<IEnumerable<LabResult>>> GetLabReports()
        {
            return await _context.LabResults
                .Include(r => r.Patient)
                .Include(r => r.Test)
                .ToListAsync();
        }

        #endregion

        #region 4. Get Reports By Patient

        public async Task<ActionResult<IEnumerable<LabResult>>> GetReportsByPatient(int patientId)
        {
            return await _context.LabResults
                .Include(r => r.Test)
                .Where(r => r.PatientId == patientId)
                .ToListAsync();
        }

        #endregion

    }
}
