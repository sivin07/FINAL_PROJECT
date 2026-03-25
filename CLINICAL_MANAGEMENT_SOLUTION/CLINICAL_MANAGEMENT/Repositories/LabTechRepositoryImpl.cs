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
                .Include(p => p.Test)
                .Where(p => p.Status == "Pending")
                .ToListAsync();
        }

        #endregion

        #region 2. Add Lab Result
        public async Task<ActionResult<LabResult>> AddLabResult(LabResult labResult)
        {
            try
            {
                await _context.LabResults.AddAsync(labResult);
                await _context.SaveChangesAsync();

                return labResult;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region 3. Update Prescription Status
        public async Task<bool> UpdatePrescriptionStatus(int prescriptionId)
        {
            var prescription = await _context.LabTestPrescriptions.FindAsync(prescriptionId);

            if (prescription == null)
                return false;

            prescription.Status = "Completed";
            prescription.ResultDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region 4. Generate Lab Bill
        public async Task<ActionResult<LabTestResultBill>> GenerateLabBill(LabTestResultBill bill)
        {
            try
            {
                await _context.LabTestResultBills.AddAsync(bill);
                await _context.SaveChangesAsync();

                return bill;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region 5. Get All Lab Reports
        public async Task<ActionResult<IEnumerable<LabResult>>> GetLabReports()
        {
            return await _context.LabResults
                .Include(r => r.Patient)
                .Include(r => r.Test)
                .ToListAsync();
        }

        #endregion

        #region 6. Get Reports By Patient
        public async Task<ActionResult<IEnumerable<LabResult>>> GetReportsByPatient(int patientId)
        {
            return await _context.LabResults
                .Include(r => r.Test)
                .Where(r => r.PatientId == patientId)
                .ToListAsync();
        }

        #endregion

        public async Task<bool> CompleteLabTest(int prescriptionId, LabResult labResult)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // STEP 1: Get Prescription
                var prescription = await _context.LabTestPrescriptions
                    .Include(p => p.Test)
                    .FirstOrDefaultAsync(p => p.PrescriptionId == prescriptionId);

                if (prescription == null)
                    return false;

                // Defensive check (don’t allow duplicate completion)
                if (prescription.Status == "Completed")
                    return false;

                // STEP 2: Insert Lab Result
                labResult.PatientId = prescription.PatientId;
                labResult.TestId = prescription.TestId ?? 0;
                labResult.Date = DateTime.Now;

                await _context.LabResults.AddAsync(labResult);
                await _context.SaveChangesAsync();

                // STEP 3: Auto Generate Bill using LabTest.Price
                var testPrice = prescription.Test?.Price ?? 0;

                var bill = new LabTestResultBill
                {
                    ResultId = labResult.ResultId,
                    LabTestBill = testPrice
                };

                await _context.LabTestResultBills.AddAsync(bill);
                await _context.SaveChangesAsync();

                // STEP 4: Update Prescription Status
                prescription.Status = "Completed";
                prescription.ResultDate = DateTime.Now;

                await _context.SaveChangesAsync();

                // STEP 5: Commit Transaction
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

    }
}
